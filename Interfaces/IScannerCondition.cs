using VidsNet.DataModels;

namespace VidsNet.Interfaces
{
    public interface IScannerCondition {
        ScannerConditionResult CheckType(string filePath);
    }
}