using Xunit;

// Disable parallel test execution since tests share in-memory state
[assembly: CollectionBehavior(DisableTestParallelization = true)]
