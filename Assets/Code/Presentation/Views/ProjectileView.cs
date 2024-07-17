using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileView : View<ProjectileViewModel>
{
    protected override void Bind(ProjectileViewModel viewModel, ICompositeDisposable disposable, SubviewBinder subviewBinder) {
        viewModel.velocity.Subscribe(dir => transform.rotation = Quaternion.LookRotation(Vector3.forward, dir)).AddTo(disposable);
        viewModel.position.Subscribe(pos => transform.position = pos).AddTo(disposable);
    }
}
