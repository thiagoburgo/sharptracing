using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace DrawEngine.SharpTracingUI
{
    internal class UndoEngineImpl : UndoEngine
    {
        // points to the command that should be executed for Redo
        private int currentPos = 0;
        private List<UndoUnit> undoUnitList = new List<UndoUnit>();
        public UndoEngineImpl(IServiceProvider provider) : base(provider) {}
        public void DoUndo()
        {
            if(this.currentPos > 0){
                UndoUnit undoUnit = this.undoUnitList[this.currentPos - 1];
                undoUnit.Undo();
                this.currentPos--;
            }
            this.UpdateUndoRedoMenuCommandsStatus();
        }
        public void DoRedo()
        {
            if(this.currentPos < this.undoUnitList.Count){
                UndoUnit undoUnit = this.undoUnitList[this.currentPos];
                undoUnit.Undo();
                this.currentPos++;
            }
            this.UpdateUndoRedoMenuCommandsStatus();
        }
        private void UpdateUndoRedoMenuCommandsStatus()
        {
            // this components maybe cached.
            IMenuCommandService menuCommandService = this.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
            MenuCommand undoMenuCommand = menuCommandService.FindCommand(StandardCommands.Undo);
            MenuCommand redoMenuCommand = menuCommandService.FindCommand(StandardCommands.Redo);
            if(undoMenuCommand != null){
                undoMenuCommand.Enabled = this.currentPos > 0;
            }
            if(redoMenuCommand != null){
                redoMenuCommand.Enabled = this.currentPos < this.undoUnitList.Count;
            }
        }
        protected override void AddUndoUnit(UndoUnit unit)
        {
            this.undoUnitList.RemoveRange(this.currentPos, this.undoUnitList.Count - this.currentPos);
            this.undoUnitList.Add(unit);
            this.currentPos = this.undoUnitList.Count;
        }
        protected override UndoUnit CreateUndoUnit(string name, bool primary)
        {
            return base.CreateUndoUnit(name, primary);
        }
        protected override void DiscardUndoUnit(UndoUnit unit)
        {
            this.undoUnitList.Remove(unit);
            base.DiscardUndoUnit(unit);
        }
        protected override void OnUndoing(EventArgs e)
        {
            base.OnUndoing(e);
        }
        protected override void OnUndone(EventArgs e)
        {
            base.OnUndone(e);
        }
    }
}