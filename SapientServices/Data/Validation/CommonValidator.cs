// Crown-owned copyright, 2021-2024
using FluentValidation;
using Sapient.Data;
using System;

namespace SapientServices.Data.Validation
{
    internal static class CommonValidator
    {
        public static IRuleBuilderOptions<T, string> IsValidUuid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {

            return ruleBuilder
                .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out var _))
                .WithMessage("Invalid UUID");
        }
        public static IRuleBuilderOptions<T, string> IsValidUlid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {

            return ruleBuilder
                .Must(x => string.IsNullOrWhiteSpace(x) || Ulid.TryParse(x, out var _))
                .WithMessage("Invalid ULID");
        }

        public static IRuleBuilderOptions<T, string> IsValidUuidOrUlid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => string.IsNullOrWhiteSpace(x) || Guid.TryParse(x, out var _) || Ulid.TryParse(x, out var r))
                .WithMessage("Not a valid ULID or UUID");
        }

        public static bool IsValidUlid(string ulid)
        {
            bool result = true;
            if (string.IsNullOrWhiteSpace(ulid) || !Ulid.TryParse(ulid, out var _))
            {
                result = false;
            }
            return result;
        }

        public static bool BeAValidScalar(float number)
        {
            return number >= 0.0 && number <= 1.0;
        }

        public static bool BeAValidClassification(DetectionReport.Types.DetectionReportClassification classType)
        {
            var classObject = Classifications.FirstOrDefault(c => c.ClassValue.Equals(classType.Type, StringComparison.OrdinalIgnoreCase));

            // Class type not found then return false
            if (classObject == null)
            {
                return false;
            }

            // If no SubClass type added and first case passed return true 
            if (classType.SubClass == null || !classType.SubClass.Any())
            {
                return true;
            }

            var subClassL1 = classType.SubClass.Select(sc => sc.Type).ToList();
            var inSubClasses = classObject.SubClassL1.Where(sc => subClassL1.Contains(sc.SubClassL1Value, StringComparer.OrdinalIgnoreCase));

            // Sub Class type not found / all Sub Class types not matched
            if (!inSubClasses.Any() && inSubClasses.Count() != classType.SubClass.Count())
            {
                return false;
            }

            var l2SubClassTypeList = new List<string>();

            foreach (var l1SubClass in classType.SubClass)
            {
                if (l1SubClass.SubClass_ != null && l1SubClass.SubClass_.Any())
                {
                    l2SubClassTypeList.AddRange(l1SubClass.SubClass_.Select(t => t.Type));
                }
            }

            // Level 2 Sub Class not existin then exit.
            if (l2SubClassTypeList.Count() <= 0)
            {
                return true;
            }

            var inL2SubClasses = classObject.SubClassL1.SelectMany(sc => sc.SubClassL2);

            // Level 2 Invalid Sub Class then exit.
            if (l2SubClassTypeList.Count(t => inL2SubClasses.Contains(t)) < l2SubClassTypeList.Count())
            {
                return false;
            }

            return true;
        }

        public static List<ClassificationModel> Classifications =>
            new List<ClassificationModel>
            {
                new ClassificationModel
                {
                    ClassValue = "Unknown",
                },
                new ClassificationModel
                {
                    ClassValue = "Human",
                    SubClassL1 = new List<SubClassLevel1Model>
                    {
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Male",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Female",
                        },
                    }
                },
                new ClassificationModel
                {
                    ClassValue = "Animal",
                    SubClassL1 = new List<SubClassLevel1Model>
                    {
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Bird",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Horse",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Other",
                        },
                    }
                },
                new ClassificationModel
                {
                    ClassValue = "Fixed Object",
                    SubClassL1 = new List<SubClassLevel1Model>
                    {
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Building",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Fixed Tower/mast",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Road",
                        },new SubClassLevel1Model
                        {
                            SubClassL1Value = "Bridge",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Dam",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Street Furniture",
                        },new SubClassLevel1Model
                        {
                            SubClassL1Value = "Natural feature",
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Other",
                        },
                    }
                },
                new ClassificationModel
                {
                    ClassValue = "Equipment",
                    SubClassL1 = new List<SubClassLevel1Model>
                    {
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "RF Transmitter",
                            SubClassL2 = new List<string>
                            {
                                "Wifi AP",
                                "Broadcast",
                                "UAS Control Station",
                                "UAV Transmitter",
                                "Radar",
                                "RF Jammer",
                                "Laser/Lidar",
                                "RF DEW/ EM Pulse",
                                "Other",
                            },
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Weapon",
                            SubClassL2 = new List<string>
                            {
                                "Gun",
                                "Missile Launcher",
                                "Missile",
                                "Bomb",
                                "IED/mine",
                                "Grenade",
                                "Other",
                            },
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Passive Sensor",
                            SubClassL2 = new List<string>
                            {
                                "Camera",
                                "Acoustic",
                                "Other",
                            },
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Other",
                        }
                    }
                },
                new ClassificationModel
                {
                    ClassValue = "Land Vehicle ",
                    SubClassL1 = new List<SubClassLevel1Model>
                    {
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Tracked",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = ">2 wheels - Heavy",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = ">2 wheels - Medium",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = ">2 wheels - Light",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "2 wheels",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                    }
                },
                new ClassificationModel
                {
                    ClassValue = "Sea Vessel",
                    SubClassL1 = new List<SubClassLevel1Model>
                    {
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Large Ship",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Small boat",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Submarine",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                    }
                },
                new ClassificationModel
                {
                    ClassValue = "Air Vehicle",
                    SubClassL1 = new List<SubClassLevel1Model>
                    {
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Manned Rotary Wing",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "Manned Fixed Wing",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "UAV Rotary Wing",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "UAV Fixed Wing",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                        new SubClassLevel1Model
                        {
                            SubClassL1Value = "UAS Control Station",
                            SubClassL2 = SubClass2CommercialMilitary,
                        },
                    }
                },
            };

        private static List<string> SubClass2CommercialMilitary =>
            new List<string>
            {
                "Commercial",
                "Military",
            };
    }

    /// <summary>
    /// The classification model.
    /// </summary>
    internal class ClassificationModel
    {
        public string ClassValue { get; set; }
        public List<SubClassLevel1Model> SubClassL1 { get; set; }
    }

    /// <summary>
    /// The sub class level1 model.
    /// </summary>
    internal class SubClassLevel1Model
    {
        public string SubClassL1Value { get; set; }

        public List<string> SubClassL2 { get; set; }
    }
}
