# GunFire Heroes - Unity开发文档

## 项目概述
GunFire Heroes是一款Unity 2D横版微信小游戏，基于README.md中的游戏设计文档开发。

## 项目结构

### 核心系统架构
```
Assets/Scripts/
├── Core/                    # 核心系统
│   ├── GameManager.cs       # 游戏主管理器
│   └── PlayerDataManager.cs # 玩家数据管理
├── Character/               # 角色系统
│   ├── CharacterManager.cs  # 角色管理器
│   └── CharacterData.cs     # 角色数据定义
├── Weapon/                  # 武器系统
│   ├── WeaponManager.cs     # 武器管理器
│   └── WeaponData.cs        # 武器数据定义
├── Economy/                 # 经济系统
│   ├── EconomyManager.cs    # 经济管理器
│   └── EconomyData.cs       # 经济数据定义
├── Gameplay/                # 游戏玩法
│   ├── GameplayManager.cs   # 玩法管理器
│   └── GameplayData.cs      # 玩法数据定义
├── Social/                  # 社交系统
│   ├── SocialManager.cs     # 社交管理器
│   └── SocialData.cs        # 社交数据定义
├── UI/                      # 用户界面
│   └── UIManager.cs         # UI管理器
└── Utils/                   # 工具类
    ├── ObjectPool.cs        # 对象池
    └── AudioManager.cs      # 音频管理器
```

### 资源结构
```
Assets/
├── Scenes/                  # 场景文件
│   └── MainScene.unity      # 主场景
├── Prefabs/                 # 预制体
│   ├── Characters/          # 角色预制体
│   ├── Weapons/             # 武器预制体
│   ├── UI/                  # UI预制体
│   └── Effects/             # 特效预制体
├── Sprites/                 # 精灵图片
│   ├── Characters/          # 角色图片
│   ├── Weapons/             # 武器图片
│   ├── UI/                  # UI图片
│   └── Effects/             # 特效图片
├── Audio/                   # 音频文件
│   ├── BGM/                 # 背景音乐
│   └── SFX/                 # 音效
├── Materials/               # 材质
├── Resources/               # 资源文件
└── StreamingAssets/         # 流媒体资源
```

## 核心功能实现

### 1. 角色系统
- **角色获取**: 7日登录免费S角色，炼狱副本掉落碎片
- **升星系统**: 消耗碎片升星，S→SS需60碎片，等差递增
- **品质突破**: 三星时可突破，原生SR比突破角色高5%属性

### 2. 经济系统
- **充值系统**: 支持微信支付，首充双倍
- **礼包系统**: 首充礼包、随机礼包
- **货币管理**: 金币、钻石、排位币、匹配代币

### 3. 武器系统
- **武器强化**: 消耗材料和金币强化武器
- **武器装备**: 支持武器切换和装备

### 4. 游戏玩法
- **关卡系统**: 单人关卡，逐步解锁
- **副本系统**: 角色碎片、武器材料、金币副本
- **PVP排位**: 实时对战，奖励排位币
- **双人匹配**: 时间限定8:00-23:00，随机关卡

### 5. 社交系统
- **排行榜**: 好友排行榜，支持微信API
- **分享功能**: 成就分享、高分分享、邀请分享

## 微信小游戏优化

### 1. 包体控制
- 首包≤4MB限制
- 资源动态加载
- 纹理压缩优化

### 2. 性能优化
- 对象池管理弹幕/敌人
- DrawCall控制
- 内存监控

### 3. 微信API集成
- 登录验证
- 支付系统
- 社交功能

## 开发指南

### 环境要求
- Unity 2022.3.0f1 或更高版本
- 支持WebGL构建
- 微信开发者工具

### 构建设置
1. 平台设置为WebGL
2. 优化设置：
   - 启用静态合批
   - 关闭动态合批
   - 纹理压缩格式设置

### 数据持久化
- 使用PlayerPrefs存储玩家数据
- JSON序列化复杂数据结构
- 自动保存机制

### 扩展开发
1. 添加新角色：创建CharacterConfig ScriptableObject
2. 添加新武器：创建WeaponConfig ScriptableObject
3. 添加新关卡：创建StageConfig ScriptableObject
4. 添加新副本：创建DungeonConfig ScriptableObject

## 注意事项

### 微信小游戏限制
- 包体大小限制
- 内存使用限制
- API调用限制

### 性能考虑
- 避免频繁GC
- 合理使用对象池
- 优化渲染批次

### 商业化合规
- 抽卡概率公示
- 支付安全验证
- 用户数据保护

## 后续开发计划

### 第一阶段
- [ ] 完善UI界面
- [ ] 实现基础战斗系统
- [ ] 添加音效和特效

### 第二阶段
- [ ] 集成微信API
- [ ] 实现网络功能
- [ ] 添加更多角色和武器

### 第三阶段
- [ ] 性能优化
- [ ] 测试和调试
- [ ] 发布准备

## 联系信息
开发团队：GunFire Heroes Studio
项目开始时间：2025-08-18