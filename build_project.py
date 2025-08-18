#!/usr/bin/env python3
"""
GunFire Heroes Unity项目构建脚本
用于自动化构建WebGL版本，适配微信小游戏
"""

import os
import sys
import subprocess
import json
import shutil
from pathlib import Path

class UnityProjectBuilder:
    def __init__(self, project_path):
        self.project_path = Path(project_path)
        self.build_path = self.project_path / "Build"
        self.webgl_build_path = self.build_path / "WebGL"
        
    def check_unity_installation(self):
        """检查Unity是否安装"""
        unity_paths = [
            "/Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/MacOS/Unity",
            "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.0f1\\Editor\\Unity.exe",
            "/opt/unity/Editor/Unity"
        ]
        
        for path in unity_paths:
            if os.path.exists(path):
                return path
                
        print("错误: 未找到Unity安装路径")
        print("请确保Unity 2022.3.0f1已正确安装")
        return None
    
    def prepare_build_directory(self):
        """准备构建目录"""
        if self.build_path.exists():
            shutil.rmtree(self.build_path)
        self.build_path.mkdir(parents=True, exist_ok=True)
        self.webgl_build_path.mkdir(parents=True, exist_ok=True)
        
    def create_build_script(self):
        """创建Unity构建脚本"""
        build_script = '''
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildScript
{
    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        string buildPath = "Build/WebGL";
        
        // 确保构建目录存在
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }
        
        // 设置构建选项
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/MainScene.unity" };
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;
        
        // 设置WebGL特定选项
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.memorySize = 16;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        PlayerSettings.WebGL.dataCaching = true;
        
        // 开始构建
        Debug.Log("开始构建WebGL版本...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("构建成功! 大小: " + summary.totalSize + " bytes");
        }
        else
        {
            Debug.LogError("构建失败!");
        }
    }
    
    public static void BuildFromCommandLine()
    {
        BuildWebGL();
        EditorApplication.Exit(0);
    }
}
'''
        
        editor_path = self.project_path / "Assets" / "Editor"
        editor_path.mkdir(exist_ok=True)
        
        with open(editor_path / "BuildScript.cs", "w", encoding="utf-8") as f:
            f.write(build_script)
    
    def build_webgl(self, unity_path):
        """构建WebGL版本"""
        print("开始构建WebGL版本...")
        
        cmd = [
            unity_path,
            "-batchmode",
            "-quit",
            "-projectPath", str(self.project_path),
            "-executeMethod", "BuildScript.BuildFromCommandLine",
            "-logFile", str(self.build_path / "build.log")
        ]
        
        try:
            result = subprocess.run(cmd, capture_output=True, text=True, timeout=1800)  # 30分钟超时
            
            if result.returncode == 0:
                print("WebGL构建成功!")
                return True
            else:
                print(f"构建失败，返回码: {result.returncode}")
                print(f"错误输出: {result.stderr}")
                return False
                
        except subprocess.TimeoutExpired:
            print("构建超时!")
            return False
        except Exception as e:
            print(f"构建过程中发生错误: {e}")
            return False
    
    def optimize_for_wechat(self):
        """为微信小游戏优化构建结果"""
        print("为微信小游戏优化构建结果...")
        
        # 检查构建文件
        build_files = list(self.webgl_build_path.glob("*"))
        if not build_files:
            print("错误: 未找到构建文件")
            return False
        
        # 创建微信小游戏配置文件
        game_json = {
            "deviceOrientation": "landscape",
            "showStatusBar": False,
            "networkTimeout": {
                "request": 60000,
                "connectSocket": 60000,
                "uploadFile": 60000,
                "downloadFile": 60000
            },
            "subpackages": []
        }
        
        with open(self.webgl_build_path / "game.json", "w", encoding="utf-8") as f:
            json.dump(game_json, f, indent=2, ensure_ascii=False)
        
        # 创建项目配置文件
        project_config = {
            "description": "GunFire Heroes - 横版射击小游戏",
            "setting": {
                "urlCheck": False,
                "es6": True,
                "enhance": True,
                "postcss": True,
                "minified": True
            },
            "compileType": "miniprogram",
            "libVersion": "2.14.1",
            "appid": "your_app_id_here",
            "projectname": "GunFire Heroes",
            "condition": {}
        }
        
        with open(self.webgl_build_path / "project.config.json", "w", encoding="utf-8") as f:
            json.dump(project_config, f, indent=2, ensure_ascii=False)
        
        print("微信小游戏优化完成!")
        return True
    
    def create_readme(self):
        """创建构建说明文件"""
        readme_content = """# GunFire Heroes - WebGL构建说明

## 构建信息
- 构建时间: 自动生成
- Unity版本: 2022.3.0f1
- 目标平台: WebGL (微信小游戏)

## 部署说明

### 微信小游戏部署
1. 使用微信开发者工具打开Build/WebGL目录
2. 配置game.json中的相关设置
3. 上传代码并提交审核

### 本地测试
1. 启动本地HTTP服务器
2. 访问index.html文件
3. 注意：由于CORS限制，需要通过HTTP服务器访问

## 优化建议
- 首包大小控制在4MB以内
- 使用资源分包加载
- 启用Gzip压缩
- 优化纹理和音频资源

## 注意事项
- 确保所有资源路径正确
- 检查微信API调用
- 测试支付和分享功能
"""
        
        with open(self.webgl_build_path / "README.md", "w", encoding="utf-8") as f:
            f.write(readme_content)
    
    def build(self):
        """执行完整构建流程"""
        print("=== GunFire Heroes Unity项目构建 ===")
        
        # 检查Unity安装
        unity_path = self.check_unity_installation()
        if not unity_path:
            return False
        
        # 准备构建目录
        self.prepare_build_directory()
        
        # 创建构建脚本
        self.create_build_script()
        
        # 构建WebGL
        if not self.build_webgl(unity_path):
            return False
        
        # 微信小游戏优化
        if not self.optimize_for_wechat():
            return False
        
        # 创建说明文件
        self.create_readme()
        
        print("=== 构建完成 ===")
        print(f"构建文件位置: {self.webgl_build_path}")
        print("请使用微信开发者工具打开构建目录进行测试")
        
        return True

def main():
    if len(sys.argv) > 1:
        project_path = sys.argv[1]
    else:
        project_path = os.getcwd()
    
    builder = UnityProjectBuilder(project_path)
    success = builder.build()
    
    sys.exit(0 if success else 1)

if __name__ == "__main__":
    main()