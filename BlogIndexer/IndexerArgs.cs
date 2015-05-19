using PowerArgs;

namespace BlogIndexer
{
    internal class IndexerArgs
    {
        [ArgRequired(PromptIfMissing = true)]
        [ArgExistingDirectory]
        [ArgShortcut("d")] 
        public string PostDirectory { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("i")] 
        public string IndexName { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("s")] 
        public string SearchService { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        [ArgShortcut("k")] 
        public string Key { get; set; }
    }
}