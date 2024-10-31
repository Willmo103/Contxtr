namespace Contxtr.Core.Configuration
{
    public class ProcessingConfig
    {
        public bool ParallelProcessing { get; set; } = true;
        public int MaxDegreeOfParallelism { get; set; } = -1; // -1 means use all available processors
        public bool FollowSymlinks { get; set; } = false;
    }
}
