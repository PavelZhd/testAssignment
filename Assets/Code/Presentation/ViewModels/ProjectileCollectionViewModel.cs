public delegate ProjectileCollectionViewModel ProjectileCollectionViewModelFactory();
public class ProjectileCollectionViewModel : DynamicCollectionViewModel<ProjectileViewModel>
{
    public ProjectileCollectionViewModel(
        IReadOnlyModel readOnlyModel,
        ProjectileViewModelFactory itemFactory
        ) : base(readOnlyModel.changed, () => readOnlyModel.getActiveProjectiles, id => itemFactory(id)) {

    }
}