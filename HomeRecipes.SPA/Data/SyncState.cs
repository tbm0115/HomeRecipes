
namespace HomeRecipes.SPA.Data
{
    public class SyncState
    {
        public bool IsSynchronizing { get; private set; }
        public DateTime? LastUpdated { get; private set; }
        public bool IsSynced { get; private set; }
        public bool LastSyncFailed { get; private set; }
        public event Action OnComplete;

        //private LocalRecipeStore LocalRecipeStore { get; set; }

        public SyncState() { }

        public bool SetSyncState(bool state) {
            IsSynced = state;
            NotifyStateChanged();
            return IsSynced;
        }

        public async Task<bool> Synchronize(LocalRecipeStore localRecipeStore) {
            bool response;
            try {
                await localRecipeStore.SynchronizeAsync();
                response = true;
            } catch (Exception ex) {
                Console.WriteLine(ex);
                response = false;
            }

            return SetSyncState(response);
        }

        private void NotifyStateChanged() => OnComplete?.Invoke();
    }
}
