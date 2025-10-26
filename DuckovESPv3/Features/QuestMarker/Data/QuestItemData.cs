namespace DuckovESPv3.Features.QuestMarker.Data
{
    /// <summary>
    /// 任务物品数据
    /// </summary>
    public class QuestItemData
    {
        /// <summary>
        /// 物品TypeID（Item.TypeID / ItemMetaData.id）
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 物品显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 需要收集的数量
        /// </summary>
        public int RequiredCount { get; set; }

        /// <summary>
        /// 物品类型（例如：武器、工具、建筑材料等）
        /// </summary>
        public string ItemType { get; set; } = string.Empty;

        /// <summary>
        /// 是否为建筑材料
        /// </summary>
        public bool IsBuildingMaterial { get; set; }

        /// <summary>
        /// 关联的任务ID
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string QuestDescription { get; set; } = string.Empty;
    }
}
