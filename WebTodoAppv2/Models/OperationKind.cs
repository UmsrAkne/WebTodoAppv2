namespace WebTodoAppv2.Models
{
    public enum OperationKind
    {
        /// <summary>
        /// 作業開始を表します
        /// </summary>
        Start,

        /// <summary>
        /// 作業を中断したことを表します
        /// </summary>
        Pause,

        /// <summary>
        /// 作業を中断後、再開したことを表します
        /// </summary>
        Resume,

        /// <summary>
        /// 作業を完了したことを表します
        /// </summary>
        Complete,

        /// <summary>
        /// 完了済みの作業を未完了に戻したことを表します
        /// </summary>
        SwitchToIncomplete,
    }
}
