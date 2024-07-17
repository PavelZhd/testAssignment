using System.Collections;
using System.Collections.Generic;

public interface IView
{
    void Bind(IViewModel viewModel, ICompositeDisposable unbinder, SubviewBinder subviewBinder);
}

public interface IViewModel: System.IDisposable { }

public interface IBind
{
    void Bind(IViewModel viewModel, SubviewBinder subviewBinder);
}