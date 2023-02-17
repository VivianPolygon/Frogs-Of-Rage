public interface IVacuumState
{
    void HandleAiState(VacuumNavigation vacuumNavigation);

    void ReceiveNavigationData(VacuumNavigation vacuumNavigation);
}
