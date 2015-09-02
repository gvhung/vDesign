namespace Base.BusinessProcesses.Entities
{
    public enum FlowStepType
    {
        Step = 0,
        Template,
        Stage,
        BranchingStep,
        TaskStep,
        CreateObjectTask,
        ExtendedStage,
        RestoreStep,
        EndStep,
        WorkflowOwnerStep,
        GotoStep,
        ParalleizationStep,
        ParallelEndStep,
    }
}