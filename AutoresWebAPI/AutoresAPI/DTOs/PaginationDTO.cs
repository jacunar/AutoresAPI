namespace AutoresAPI.DTOs; 
public class PaginationDTO {
    public int Page { get; set; } = 1;
    public int recordsByPage = 10;
    private readonly int _maxSizeByPage = 50;

    public int RecordsByPage {
        get { return recordsByPage; }
        set {
            recordsByPage = (value > _maxSizeByPage) ? _maxSizeByPage : value;
        }
    }
}