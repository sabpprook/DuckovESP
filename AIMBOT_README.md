# 自动瞄准和自动扳机功能说明

## ✅ 功能已完成并集成

### 核心功能

1. **自动瞄准 (Aimbot)**
   - 按住右键（Mouse1）时自动瞄准最近的敌人
   - FOV范围内精确筛选目标
   - 平滑相机旋转，模拟真实瞄准
   - 支持头部/身体瞄准切换
   - 运动预测（提前量）

2. **自动扳机 (Trigger Bot)**
   - 当准星对准敌人时自动开火
   - 可配置延迟时间避免检测
   - 支持仅在瞄准状态（ADS）时触发
   - 队伍检查避免误伤队友

### 配置选项（按F5打开菜单）

#### 自动瞄准设置
- **启用自动瞄准**: 总开关
- **瞄准键**: 默认鼠标右键（Mouse1）
- **FOV范围**: 5-90度，默认30度
  - 较小值 = 更精确但范围小
  - 较大值 = 范围大但可能瞄向侧面敌人
- **平滑度**: 1-20，默认5
  - 较小值 = 瞄准更快但不自然
  - 较大值 = 瞄准更慢但更像真人
- **最大距离**: 50-500米，默认200米
- **瞄准头部**: 启用时瞄准头部（1.7米高），否则瞄准身体（1.0米高）
- **运动预测**: 根据敌人移动速度提前瞄准

#### 自动扳机设置
- **启用自动扳机**: 总开关
- **触发延迟**: 0-0.5秒，默认0.05秒
  - 模拟人类反应时间，避免立即开火被检测
- **仅在瞄准时**: 只在ADS（右键瞄准）状态下触发
- **队伍检查**: 避免对队友开火

### 技术细节

#### 目标选择算法
1. 扫描所有CharacterMainControl实例
2. 过滤条件：
   - 存活状态（Health > 0）
   - 队伍检查（非队友）
   - 距离检查（< 最大距离）
   - FOV检查（在视野锥内）
3. 评分系统：
   - 距离越近 = 优先级越高
   - 屏幕中心越近 = 优先级越高
4. 选择最高分目标

#### 瞄准实现
```csharp
// 计算目标方向
Vector3 direction = aimPoint - cameraPosition;
Quaternion targetRotation = Quaternion.LookRotation(direction);

// 平滑插值（Slerp）
float smoothSpeed = Time.deltaTime * smoothness;
cameraTransform.rotation = Quaternion.Slerp(current, target, smoothSpeed);
```

#### 运动预测
```csharp
// 基于敌人速度计算提前量
Vector3 velocity = enemyRigidbody.velocity;
float bulletTime = distance / bulletSpeed;  // 子弹飞行时间
Vector3 predictedPosition = currentPosition + (velocity * bulletTime);
```

#### 自动射击
```csharp
// 使用反射调用射击方法
ItemAgent_Gun gun = character.CurrentHoldItemAgent as ItemAgent_Gun;
gun?.GetType().GetMethod("TryShoot")?.Invoke(gun, null);
```

### 使用建议

#### 推荐设置（隐蔽）
- FOV: 20-30度（避免瞄向侧面）
- 平滑度: 8-15（模拟真人）
- 触发延迟: 0.1-0.2秒（自然反应时间）
- 仅在瞄准时: 启用

#### 推荐设置（激进）
- FOV: 45-60度（大范围扫描）
- 平滑度: 3-5（快速锁定）
- 触发延迟: 0.05秒（快速反应）
- 运动预测: 启用

### 安全提示

⚠️ **重要警告**：
- 此功能仅供学习和单机测试使用
- 在多人游戏中使用可能导致账号封禁
- 建议在私人服务器或单机模式下测试
- 过于明显的设置（FOV过大、平滑度过小）容易被其他玩家发现

### 故障排除

1. **瞄准不工作**
   - 检查是否按住了瞄准键（默认Mouse1）
   - 确认目标在FOV范围内
   - 检查距离是否超过最大值

2. **自动射击不触发**
   - 确认准星确实对准了敌人（Raycast检测）
   - 检查是否启用了"仅在瞄准时"且未按右键
   - 尝试增加触发延迟

3. **瞄准抖动**
   - 增加平滑度值
   - 检查帧率是否稳定

4. **射击方法未找到**
   - 游戏可能使用不同的射击方法名
   - 检查控制台日志错误信息
   - 可能需要修改`AimbotSystem.TryShoot()`的反射代码

### 文件清单

- **ESPConfig.cs**: 添加11个aimbot配置字段
- **ESPConfigMenu.cs**: 添加UI控制界面（窗口扩展到500x700）
- **AimbotSystem.cs**: 核心aimbot实现（348行）
- **ModBehaviour.cs**: 集成aimbot系统

### 开发日志

- **2024**: 初始实现
  - ✅ FOV目标筛选
  - ✅ 平滑相机旋转
  - ✅ 运动预测
  - ✅ 自动扳机系统
  - ✅ 队伍检查
  - ✅ 配置持久化

---

**注意**: 如果游戏更新后功能失效，可能需要：
1. 检查CharacterMainControl结构是否变化
2. 更新ItemAgent_Gun射击方法名
3. 检查Camera/Transform层次结构
