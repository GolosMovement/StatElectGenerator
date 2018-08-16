using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    public interface ISerializer
    {
        void CreateProtocolSet(ProtocolSet protocolSet);
        void CreateLineDescription(LineDescription lineDescription);
        void CreateProtocol(Protocol protocol);
        void CreateLineNumber(LineNumber lineNumber);
        void CreateLineString(LineString lineString);
        void AfterImport();
    }
}
