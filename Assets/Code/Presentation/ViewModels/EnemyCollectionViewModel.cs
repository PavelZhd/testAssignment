using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate EnemyCollectionViewModel EnemyCollectionViewModelFactory();
public class EnemyCollectionViewModel : DynamicCollectionViewModel<EnemyViewModel>
{
    public EnemyCollectionViewModel(
        IReadOnlyModel readOnlyModel,
        EnemyViewModelFactory itemFactory
        ) : base(readOnlyModel.changed, ()=>readOnlyModel.getActiveEnemies, id=>itemFactory(id)) 
    {

    }
}
