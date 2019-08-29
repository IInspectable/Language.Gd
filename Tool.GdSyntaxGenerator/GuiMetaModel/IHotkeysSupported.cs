using System;
using System.Collections.Generic;
using System.Text;

namespace Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator.GuiMetaModel {
    public interface IHotkeysSupported {
        void AddHotKeyToAdd(HotKey hotKey);
        void AddHotKeyToRemove(HotKey hotKey);
    }
}
