
namespace Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator.GuiMetaModel {
    public interface IContainer : IHotkeysSupported {
        Layout Layout { get; set; }
        string Name { get; set; }
        void AddGuiElement(GuiElement element);
        void RemoveGuiElement(GuiElement element);
        GuiElement[] Controls {
            get;
            set;
        }
        
    }
}