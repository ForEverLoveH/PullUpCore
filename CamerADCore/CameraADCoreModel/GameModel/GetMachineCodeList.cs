using System;
using System.Collections.Generic;

namespace CameraADCoreModel.GameModel
{
    public class GetMachineCodeList
    {
        public List<GetMachineCodeListResults> Results;

        public String Error;
    }
    public class GetMachineCodeListResults
    {
        public String title;

        public String MachineCode;
    }

    
}