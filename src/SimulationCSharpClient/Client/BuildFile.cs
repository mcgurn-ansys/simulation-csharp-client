using System;
using System.Collections.Generic;

namespace SimulationCSharpClient.Client
{
    public enum BuildFileAvailability
    {
        Uploaded = 0,
        Processing = 1,
        Available = 2,
        Error = 3
    }

    public enum BuildFileMachineType
    {
        AdditiveIndustries = 0,
        Renishaw = 1,
        SLM = 2,
        ThreeDSystems = 3
    }

    public partial class BuildFile
    {
        public BuildFileAvailability BuildFileAvailability
        {
            get
            {
                return (BuildFileAvailability)Enum.Parse(typeof(BuildFileAvailability), this.Availability);
            }

            set
            {
                this.Availability = value.ToString();
            }
        }

        public BuildFileMachineType BuildFileMachineType
        {
            get
            {
                return (BuildFileMachineType)Enum.Parse(typeof(BuildFileMachineType), this.MachineType);
            }

            set
            {
                this.MachineType = value.ToString();
            }
        }
    }
}