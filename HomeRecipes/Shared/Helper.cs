using System;
using System.Collections.Generic;
using System.Text;

namespace HomeRecipes.Shared
{
    public static class Helper
    {
        
        public static string HumanizeIngredientUnits(decimal quantity, string unitName) {
            var unit = SIUnits.UnitManager.GetUnit(unitName);
            if (unit == null) {
                return quantity.ToString();
            }

            SIUnits.Abstracts.BaseUnit convertToUnit = unit;
            switch (unit.Name) {
                case "To Taste":
                    return $"{quantity} (to taste)";
                    break;
                case "Teaspoon":
                    if (quantity >= 3) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Tablespoon");
                    }
                    break;
                case "Tablespoon":
                    if (quantity < (1/4)) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Teaspoon");
                    } else if (quantity >= 4) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Cup");
                    }
                    break;
                case "Cup":
                    if (quantity < (1/4)){
                        convertToUnit = SIUnits.UnitManager.GetUnit("Tablespoon");
                    } else if (quantity >= 2) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Pint");
                    }
                    break;
                case "Pint":
                    if (quantity < 1) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Cup");
                    } else if (quantity >= 2) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Quart");
                    }
                    break;
                case "Quart":
                    if (quantity < 1) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Pint");
                    } else if (quantity >= 4) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Gallon");
                    }
                    break;
                case "Pound":
                    if (quantity < (1/4)) {
                        convertToUnit = SIUnits.UnitManager.GetUnit("Ounce");
                    }
                    break;
                default:
                    break;
            }
            if (convertToUnit != null && convertToUnit.Symbol != unit.Symbol) {
                decimal? convertedQuantity = default;
                try {
                    convertedQuantity = SIUnits.UnitManager.Convert(unit.Format(quantity), convertToUnit.Symbol);
                } catch (Exception ex) {

                }
                if (convertedQuantity.HasValue) {
                    if (convertedQuantity.Value > 0) {
                        return HumanizeIngredientUnits(convertedQuantity.Value, convertToUnit.Name);
                    } else {
                        return convertToUnit.Format(convertedQuantity.GetValueOrDefault());
                    }
                }
            }
            return unit.Format(quantity);
        }
    }
}
