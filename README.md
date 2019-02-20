# RateLimitThrottle

基于Asp.Net Core的简易可用易扩展的限流中间件

1.支持IP策略和Client策略  
2.支持配置读文件或读库(mysql，可扩展其他库)  
3.支持白名单  
4.支持自定义扩展  

## 规则
Endpoint格式：{HTTP_Verb}:{PATH}  Http谓词和请求路径  
Period格式：{INT}{PERIOD_TYPE}   可以使用s,m,h,d  
Limit格式：{LONG}  最大次数  

匹配任意路径，1分钟50次：
```c#
{
	"Endpoint": "*",
	"Period": "1m",
	"Limit": 50
}
```

匹配/api/values的任意请求，2分钟200次：
```c#
{
	"Endpoint": "*:/api/values",
	"Period": "2m",
	"Limit": 200
}
```

匹配/api/values的Get请求，1小时1000次：
```c#
{
	"Endpoint": "get:/api/values",
	"Period": "1h",
	"Limit": 1000
}
```

同时匹配到多个规则时，取相同周期内次数最小的规则。如：  
规则1： * 20/1m  
规则2： get:/api/values  10/1m   
当请求为get:/api/values，同时满足规则1和2，在相同周期1m内，取次数最小的10次

## 基本使用
1.在Startup的ConfigureServices添加：
```c#
//如果是读取文件配置，需要加上这个，加载configuration
services.AddOptions();

//添加限流组件
services.AddRateLimiting(options =>
{
    //前缀，区分不同接口站点
    options.RateLimitCounterPrefix = "testApi1";
    //自定义日志输出
    options.LogHandler = msg =>
    {
   	    Console.WriteLine(msg);
    };
})
//内存缓存
.AddMemoryPolicyCache()
//Memory计数器
.AddMemoryCounter()
//读取文件配置
.AddDefaultPolicyStore(Configuration.GetSection("RateLimitStoreSettings"));
```
2.在Startup的Configure添加：
```c#
app.UseClientRateLimiting();//使用Client策略
//app.UseIPRateLimiting();//使用IP策略
```
策略可以同时使用，有先后顺序

## 自定义设置
1、使用Client策略  
需要根据角色判断，或者使用IdentityServer4根据ClientId判断：
```c#
services.AddRateLimiting(options =>
{
    //自定义client获取方式，根据ids4的client_id
    options.OnGetClient = httpContext =>
    {
        var client = httpContext.User?.Claims.FirstOrDefault(x => x.Type == "client_id")?.Value;
        return client;
    };
})
```
2、使用IP策略  
站点使用了CDN、负载均衡或反向代理，根据请求头获取真实IP：
```c#
services.AddRateLimiting(options =>
{
    //自定义ip获取方式, 获取nginx反向代理请求头
    options.OnGetIpAddress = httpContext =>
    {
        var ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        return ip;
    };
})
```
3、其他设置：
```c#
services.AddRateLimiting(options =>
{
    //前缀，区分不同接口站点
    options.RateLimitCounterPrefix = "testApi1";
    //策略缓存时间
    options.ProlicyCacheTime = 300;
    //限流后返回的http状态码
    options.HttpStatusCode = 429;
    //限流后返回的http消息
    options.HttpResponseFomatterMessage = "请求过于频繁";
})
```

## 自定义扩展
限流原理：  
1.读取策略配置  
2.缓存配置  
3.根据请求判断是否能匹配到对应规则  
4.如果匹配到规则，计数器累加，放行  
如果未匹配或超过最大次数，返回错误  

#### 一、读取策略配置
默认已实现读文件和读库(mysql)
```c#
//读取文件配置
AddDefaultPolicyStore(Configuration.GetSection("RateLimitStoreSettings"))
//读取mysql
.AddMySqlPolicyStore(options =>
{
    options.ConnectionString = "Server=192.168.1.19;User Id=admin;Password=123456;Database=sso;Allow User Variables=True;";
});
```
如果需要其他存储方式，只需实现IPolicyStore接口，然后注入即可

#### 二、缓存配置
默认已实现Memory缓存
如需要使用redis缓存，只需要实现ICacheManager接口，然后注入即可

#### 三、计数器
计数器已实现memory和redis   
实现接口IRateLimitCounterStore  
```c#
//Memory计数器
.AddMemoryCounter()
//Redis计数器
.AddRedisCounter(options =>
{
    options.ConnectionString = "192.168.1.19:6379,defaultDatabase=1,poolsize=50";
})
```
测试可以使用memory计数器，效率更高，但是重启会丢失  
生成环境建议使用redis计数器

## 其他
读文件配置：  [appsettings.json](https://github.com/niubileme/RateLimitThrottle/blob/master/docs/appsettings.json)  
mysql脚本：  [mysql.sql](https://github.com/niubileme/RateLimitThrottle/blob/master/docs/mysql.sql) 

## 参考
AspNetCoreRateLimit [https://github.com/stefanprodan/AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit)  
Dnc.Api.Throttle [https://github.com/kulend/Dnc.Api.Throttle](https://github.com/kulend/Dnc.Api.Throttle) 

