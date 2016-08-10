using VidsNet.DataModels;

namespace VidsNet.Interfaces
{
    public interface IScannerCondition {
        CheckTypeResult CheckType(string filePath);
    }
}