using AppForSEII2526.Web.API;
using System.Collections.ObjectModel;
namespace AppForSEII2526.Web
{
    public class PlanStateContainer : IDisposable
    {
        private readonly List<ClassForPlanDTO> _selectedClasses = new();

        // Expose the selected classes as a read-only collection
        public IReadOnlyCollection<ClassForPlanDTO> SelectedClasses => new ReadOnlyCollection<ClassForPlanDTO>(_selectedClasses);

        // PlanForCreateDTO holds the data collected across the UI pages (SelectClasses & CreatePlan)
        public PlanForCreateDTO Plan { get; set; } = new PlanForCreateDTO();

        // Event to notify components about state changes (e.g., when a class is added/removed)
        public event Action? OnChange;

        public void AddClassToPlan(ClassForPlanDTO classItem)
        {
            if (!_selectedClasses.Any(c => c.Id == classItem.Id))
            {
                _selectedClasses.Add(classItem);

                // Add a corresponding PlanItemForCreateDTO to the Plan DTO
                Plan.Items.Add(new PlanItemForCreateDTO { ClassId = classItem.Id, Goal = null });

                NotifyStateChanged();
            }
        }

        public void RemoveClassFromPlan(ClassForPlanDTO classItem)
        {
            var itemToRemove = _selectedClasses.FirstOrDefault(c => c.Id == classItem.Id);
            if (itemToRemove != null)
            {
                _selectedClasses.Remove(itemToRemove);

                // Also remove the corresponding item from the Plan DTO
                var planItemToRemove = Plan.Items.FirstOrDefault(i => i.ClassId == classItem.Id);
                if (planItemToRemove != null)
                {
                    Plan.Items.Remove(planItemToRemove);
                }

                NotifyStateChanged();
            }
        }

        public void ClearPlanCart()
        {
            _selectedClasses.Clear();
            // Reset the Plan DTO to its initial state
            Plan = new PlanForCreateDTO();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void Dispose()
        {
            OnChange = null;
        }
    }
}
