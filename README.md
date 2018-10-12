# Zoey.Dapper

### 介绍

在使用*Dapper*时通常 SQL 语句会写在类中,该库把 SQL 语句提取到文件中。

### 开始

1. 引用`Zoey.Dapper`和`Zoey.Dapper.MSSQLserver`(暂只有 MSSQLserver)项目
2. 在项目`Config`目录下添加`xml`文件

   ```xml
   <?xml version="1.0" encoding="utf-8" ?>
    <sql xmlns="http://schema.zoey.com/sql" domain="Default">
    <sql-query name="Student.GetStudentByID">
        <text>
        <![CDATA[
        SELECT * FROM Student WHERE ID = @ID
        ]]>
        </text>
    </sql-query>

    <sql-command name="Student.UpdateStudentByID">
        <text>
        <![CDATA[
        UPDATE Student SET Age = @Age,Name = @Name WHERE ID = @ID
        ]]>
        </text>
    </sql-command>
    </sql>
   ```

3. 在`Startup`中的`ConfigureServices`添加如下代码:

   ```csharp
    var physicalProvider = _env.ContentRootFileProvider;
    services.AddSingleton<IFileProvider>(physicalProvider);

    services.AddZoeyDapperCore(options =>
    {
        options.Path = new List<string>() { "/Config" };
        options.WatchFileFilter = "*.xml";
        options.StartProxy = false;
    })
    .AddMSSQLserver(option =>
    {
        option.DatabaseElements = new List<DatabaseElement>()
        {
            new DatabaseElement("Default","Data Source=.;Initial Catalog=Test;Integrated Security=True")
        };
        option.DomainElements = new List<DomainElement>()
        {
            new DomainElement()
            {
                Name = "Default",
                MasterSlaves = new MasterSlaves("Default","Default")
            }
        };
    });
   ```

4. 在`Controller`中

   1. 添加注入

   ```csharp
    private readonly ISqlContext _sqlContext;
    private readonly ISqlCommand _sqlCommand;
    public HomeController(ISqlContext sqlContext, ISqlCommand sqlCommand)
    {
        this._sqlContext = sqlContext;
        _sqlCommand = sqlCommand;
    }
   ```

   2.调用

   ```csharp
    public IActionResult Index()
    {
        var student = _sqlCommand.GetSqlElement("Student.GetStudentByID").Query<Student>(new
        {
            ID = 1
        });

        return View(student);
    }

    public IActionResult About()
    {
        var sql = _sqlContext.GetSqlElement("Student.UpdateStudentByID");
        using (var db = new SqlConnection("Data Source=.;Initial Catalog=Test;Integrated Security=True"))
        {
            db.Execute(sql.CommandText, new
            {
                Age = new Random().Next(100),
                Name = "Hello Zoey!",
                ID = 1
            });
        }
        return View();
    }
   ```
