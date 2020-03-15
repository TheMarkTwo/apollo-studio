using System.Collections.Generic;

namespace Apollo.Selection {
    public interface ISelect {
        ISelectViewer IInfo { get; }

        ISelectParent IParent { get; }
        int? IParentIndex { get; }
    }

    public interface ISelectViewer {
        void Deselect();
        void Select();
    }

    public interface ISelectParent {
        ISelectParentViewer IViewer { get; }

        List<ISelect> IChildren { get; }

        bool IRoot { get; }
    }

    public interface ISelectParentViewer {
        int? IExpanded { get; }
        void Expand(int? index);

        void Copy(int left, int right, bool cut = false);
        void Duplicate(int left, int right);
        void Paste(int right);
        void Replace(int left, int right);
        void Delete(int left, int right);

        void Group(int left, int right);
        void Ungroup(int left);
        void Choke(int left, int right);
        void Unchoke(int left);

        void Mute(int left, int right);
        void Rename(int left, int right);
        
        void Export(int left, int right);
        void Import(int right, string path = null);
    }
}