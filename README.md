# Zoey.Dapper

> 介绍

不知道大家在用`Dapper`的时候*SQL*语句是写到哪的,目前看网上的例子都是写到类里面的.

此项目的目的是把*SQL*语句放到文件(xml)中

目前只是初步版本,只是说明了意图,后面会持续完善和优化

[GitHub 地址](https://github.com/NameIsBad/Zoey.Dapper)

> 功能说明

- SQL 语句单独存放在文件(xml)中
- 支持配置多文件夹(暂不支持指定具体文件)
- 实时监听文件变化
- 支持多数据库(Dapper 本身就支持多数据,为什么这里说支持多数据呢?后面会讲到)
- 支持读写分离(功能虽然有,但配置看起来不爽,后续应该会优化. 负载平衡算法还未实现)

> 一起看看如何使用

1. 首先我们需要一个*xml*文件,如下:

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

   这里我们看下面几点:

   - `sql`节点上的`domain`属性.这里是指定数据库的连接字符串,后面会详细说明
   - 读写分离就是`sql-query`和`sql-command`节点来判断的
   - `name`属性目前是所有文件都不能重复

2. 在`Startup`中的`ConfigureServices`添加如下代码:

   ```csharp
   //SQL文件的读取和监视依赖 IFileProvider
    var physicalProvider = _env.ContentRootFileProvider;
    services.AddSingleton<IFileProvider>(physicalProvider);

    services.AddZoeyDapperCore(options =>
    {
        //SQL文件夹的路径  支持多个文件夹
        options.Path = new List<string>() { "/Config" };
        //监控文件的后缀(默认未 *.*)
        options.WatchFileFilter = "*.xml";
        //该属性暂时没用
        options.StartProxy = false;
    })
    //添加MSSQLserver
    //这里说明一下 该方法是非必要的,下面会说
    .AddMSSQLserver(option =>
    {
        //添加数据库连接字符串
        //这里为什么没用配置文件读取,考虑到可能用到(Secret Manager)
        //后面可以提供直接从配置文件中读取
        option.DatabaseElements = new List<DatabaseElement>()
        {
            //参数1:唯一名称
            //参数2:连接字符串
            new DatabaseElement("TESTDB","Data Source=.;Initial Catalog=Test;Integrated Security=True")
        };
        //此处就是上面提到的 domain 节点
        //每个domain对象有个唯一名称(xml文件domain的节点)
        //每个domain对象都有 Master(主库) 和 Slave(从库) 的名称(上面配置信息的名称)
        option.DomainElements = new List<DomainElement>()
        {
            new DomainElement()
            {
                //xml文件domain节点的名称
                Name = "Default",
                //主库和从库的名称(上面配置信息的名称)
                //主库和从库可配多个(负载均衡算法暂没实现)
                MasterSlaves = new MasterSlaves("TESTDB","TESTDB")
            }
        };
    });
   ```

   说明:

   - 大家也看到了,此处的配置很是繁琐,上面我也说了,这里应该要优化(但不紧急)

3. 用的时候有两种方式.

   1. 当我们没调用`AddMSSQLserver`时,在`Controller`中:

      ```csharp
          public class HomeController : Controller
          {
              //注入ISqlContext
              private readonly ISqlContext _sqlContext;
              public HomeController(ISqlContext sqlContext)
              {
                  this._sqlContext = sqlContext;
              }

              public IActionResult Index()
              {
                  List<Student> student;
                  //获取 Student.GetStudentByID SQL信息
                  var sql = _sqlContext.GetSqlElement("Student.GetStudentByID");
                  using (var db = new SqlConnection("Data Source=.;Initial Catalog=Test;Integrated Security=True"))
                  {
                      student = db.Query<Student>(sql.CommandText, new
                      {
                          ID = 1
                      });
                  }
                  return View(student);
              }

              public IActionResult About()
              {
                  //获取 Student.GetStudentByID SQL信息
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
          }
      ```

      说明:此种方法只是单纯的获取`SQL`信息,没什么特别的.

   2. 下面我们看第二种:

      ```csharp
          public class HomeController : Controller
          {
              //注入 ISqlCommand
              private readonly ISqlCommand _sqlCommand;
              public HomeController(ISqlCommand sqlCommand)
              {
                  _sqlCommand = sqlCommand;
              }

              public IActionResult Index()
              {
                  //此处直接执行 Query 方法
                  var student = _sqlCommand.GetSqlElement("Student.GetStudentByID").Query<Student>(new
                  {
                      ID = 1
                  });

                  return View(student);
              }
          }
      ```

      说明:

      - 用此种方法必须在 `Startup`中调用`AddMSSQLserver`方法
      - 实现了读写分离,当然 如果主库和从库连接字符串一样就效果了
      - 这就是为什么说*支持多数据库的原因了*
