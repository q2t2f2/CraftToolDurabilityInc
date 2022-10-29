return {
	BackendPlugins = 
	{
		[1] = [[MoreFoodCreateBackend.dll]]
	},
	Author = [[熟悉的总督]],
	DefaultSettings = 
	{
		[1] = 
		{
			DisplayName = [[工具最大耐久]],
			MaxValue = 200,
			Key = [[CraftToolDurability]],
			DefaultValue = 100,
			MinValue = 0,
			Description = [[修改工具的最大耐久，设成0表示不修改。]],
			SettingType = [[Slider]]
		},
		[2] = 
		{
			DisplayName = [[工具耐久不会减少]],
			Key = [[NoCraftToolDurabilityReduce]],
			DefaultValue = true,
			Description = [[工具耐久不会减少，获得的是原有耐久和设置最大耐久的较大值。]],
			SettingType = [[Toggle]]
		},
		[3] = 
		{
			DisplayName = [[制造物品立即完成]],
			Key = [[MakeImmediatelyComplete]],
			DefaultValue = true,
			Description = [[制造物品立即完成。]],
			SettingType = [[Toggle]]
		}
	},
	Source = 1,
	Cover = [[Cover.png]],
	FileId = 2880232879,
	Description = [[修改工具最大耐久,一键最大个数，方便批量制造食物和药品。
-----
V0.2 修复一键最大按钮显示状态的问题。]],
	Version = [[2]],
	Title = [[工具最大耐久,一键最大个数]],
	FrontendPlugins = 
	{
		[1] = [[MoreFoodCreateFrontend.dll]]
	}
}