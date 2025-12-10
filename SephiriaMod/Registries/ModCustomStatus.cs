using SephiriaMod.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaMod.Registries
{
    public class ModCustomStatus
    {
        public static ModCustomStatus CreateStatus(string name)
        {
            return new ModCustomStatus().SetStatus(name);
        }
        internal ModCustomStatus SetStatus(string name)
        {
            Name = name;
            StatKeyword = name;
            ClassName = "StatusInstance_Custom/" + name;
            Id = name.ToFileNameUpper();
            return this;
        }
        public string Name { get; internal set; }
        public string ClassName { get; internal set; }
        public string Id { get; internal set; }
        public bool DisplayValue { get; internal set; } = true;
        public int DivideForDisplay { get; internal set; } = 1;
        public bool IncludePositiveNegativeSign { get; internal set; } = true;
        public LocalizedString LocalizedSymbol { get; internal set; } = new LocalizedString();
        public bool UseLocalizedSymbol { get; internal set; }
        public string Symbol { get; internal set; } = "";
        public string StatKeyword { get; internal set; }
        public StatusEntity StatusEntity { get; internal set; }

        public void Init()
        {
            StatusEntity = CreateStatusEntity();
        }
        public StatusEntity CreateStatusEntity()
        {
            var entity = ScriptableObject.CreateInstance<StatusEntity>();
            entity.name = Name;
            entity.id = Id;
            entity.displayValue = DisplayValue;
            entity.divideForDisplay = DivideForDisplay;
            entity.className = ClassName;
            entity.includePositiveNegativeSign = IncludePositiveNegativeSign;
            entity.localizedSymbol = LocalizedSymbol;
            entity.useLocalizedSymbol = UseLocalizedSymbol;
            entity.symbol = Symbol;
            entity.statKeyword = StatKeyword;
            return entity;
        }
    }
}
