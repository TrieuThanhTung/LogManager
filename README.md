# Log Manager Application

**Log Manager** là ứng dụng Windows Forms (.NET 9) giúp quản lý, lọc và trích xuất file log từ hệ thống Local và SFTP Server một cách hiệu quả.

## Tính Năng Chính
- **Đa Nguồn Dữ Liệu**: Hỗ trợ đọc file từ ổ cứng (Local) hoặc kết nối SFTP an toàn (SSH.NET).
- **Bộ Lọc Thông Minh**:
  - **Theo Ngày**: Lọc file trong khoảng thời gian cụ thể.
  - **Theo Trạng Thái**: Tự động nhận diện PASS/FAIL qua regex cấu hình động.
  - **Theo MAC Address**: Lọc file liên quan đến danh sách thiết bị (MAC).
  - **Smart Newest**: Tùy chọn lấy file mới nhất (Toàn cục hoặc Theo từng MAC).
- **Tìm Kiếm Đệ Quy**: Quét toàn bộ thư mục con.
- **Batch Export**: Sao chép/Tải hàng loạt file đã lọc ra thư mục đích.
- **Cấu Hình Linh Hoạt**: Thay đổi thông số SFTP và Regex ngay trên giao diện (Runtime Config).

## Cài Đặt & Chạy
### Yêu cầu
- .NET 9.0 SDK trở lên.
- Windows OS.

### Chạy ứng dụng
Mở terminal tại thư mục dự án và chạy:
```bash
dotnet run
```
Hoặc mở file `LogManager.sln` bằng Visual Studio.

## Cấu Hình (Appsettings)
Mọi cấu hình được lưu trong `appsettings.json`. Bạn có thể sửa thủ công hoặc dùng menu **Settings** trong app.

```json
{
  "AppConfiguration": {
    "PassPattern": "^PASS_",
    "FailPattern": "^FAIL_",
    "Sftp": {
      "Host": "192.168.1.x",
      "Username": "admin",
      ...
    }
  }
}
```

## Kiến Trúc & Thiết Kế (Architecture & Design)

### 1. Class Diagram
Hệ thống sử dụng **Layered Architecture** và **Dependency Injection**.

```mermaid
classDiagram
    direction TB
    
    class MainForm {
        -IFileSourceProvider _currentProvider
        -ILogFilterStrategy _filterStrategy
        -MacAddressProvider _macProvider
        +LoadLogs()
        +ExportFiles()
        +OpenSettings()
    }

    class IFileSourceProvider {
        <<interface>>
        +GetFilesAsync(path, recursive)
        +GetFileStreamAsync(path)
    }

    class LocalFileProvider {
        +GetFilesAsync()
        +GetFileStreamAsync()
    }
    
    class SftpFileProvider {
        -SftpConfiguration _config
        +ConnectAsync()
        +GetFilesAsync()
        +GetFileStreamAsync()
    }

    class ILogFilterStrategy {
        <<interface>>
        +ApplyFilterAsync(files, options)
    }

    class LogFilterService {
        -AppConfiguration _config
        +ApplyFilterAsync()
        -FilterByStatus()
        -FilterByMac()
        -FilterByDate()
        -FilterNewestSmart()
    }

    class MacAddressProvider {
        +GetMacAddressesAsync(path)
    }

    class ConfigManager {
        +LoadConfig()
        +SaveConfig()
    }

    MainForm --> IFileSourceProvider : Uses
    MainForm --> ILogFilterStrategy : Uses
    MainForm --> MacAddressProvider : Uses
    MainForm --> ConfigManager : Uses
    
    IFileSourceProvider <|.. LocalFileProvider : Implements
    IFileSourceProvider <|.. SftpFileProvider : Implements
    ILogFilterStrategy <|.. LogFilterService : Implements
```

### 2. Luồng Xử Lý Chính (Core Flow)

**Load & Filter Process:**
```mermaid
sequenceDiagram
    actor User
    participant UI as MainForm
    participant Provider as IFileSourceProvider
    participant Filter as LogFilterService
    
    User->>UI: Click "Load & Filter"
    
    alt Source = Local
        UI->>Provider: LocalFileProvider.GetFilesAsync()
    else Source = SFTP
        UI->>Provider: SftpFileProvider.GetFilesAsync()
    end
    
    Provider-->>UI: List<FileMetadata> (All Files)
    
    UI->>Filter: ApplyFilterAsync(files, options)
    activate Filter
    Filter->>Filter: Apply Dates & Status Regex
    Filter->>Filter: Apply MAC Filter
    
    alt OnlyNewest = True
        alt Has MAC List
            Filter->>Filter: Group by MAC -> Take Top 1 each
        else No MAC List
            Filter->>Filter: Take Top 1 Global
        end
    end
    
    Filter-->>UI: Filtered List
    deactivate Filter
    
    UI->>DataGrid: Display Results
```

**Export Process:**
```mermaid
sequenceDiagram
    actor User
    participant UI as MainForm
    participant Provider as IFileSourceProvider
    participant Disk as Local Disk
    
    User->>UI: Click "EXPORT FILES" & Select Folder
    
    loop For Each File
        UI->>Provider: GetFileStreamAsync(file.FullPath)
        Provider-->>UI: Stream (Source)
        UI->>Disk: Copy Stream to Destination
    end
    
    UI->>User: "Export Complete"
```
