namespace WebTodoAppv2.Models
{
    public enum WorkingState
    {
        /// <summary>
        /// 作業に未着手の状態です
        /// </summary>
        InitialState,

        /// <summary>
        /// 作業中の状態です
        /// </summary>
        Working,

        /// <summary>
        /// 休憩中です
        /// </summary>
        Pausing,

        /// <summary>
        /// 作業が完了している状態です
        /// </summary>
        Completed,
    }
}
