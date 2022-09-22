@echo off
::获取管理员权限
%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit
::启动consul
start cmd /k "cd /d E:\daimawudong\anzhuangbao\consul_1.12.3_windows_386&&consul agent -dev -client=0.0.0.0"
::启动网关
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACormmerce.OcelotGateway\DaiMaWuDong.MSACormmerce.OcelotGateway&&dotnet run --urls="http://*:6299" --ip="127.0.0.1" --port=6299"
ping 127.0.0.1 -n 6 >nul
::启动鉴权中心
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.AuthenticationCenter\DaiMaWuDong.MSACommerce.AuthenticationCenter&&dotnet run --urls="http://localhost:7200" ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=7200 ConsulRegisterOption:GroupName=AuthenticationCenter ConsulRegisterOption:HealthCheckUrl=http://localhost:7200/Health ConsulRegisterOption:Tag=13"
::启动用户微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.Core.UserApi\DaiMaWuDong.Core.UserApi&&dotnet run --urls="http://localhost:5726" --ip="localhost" --port=5726 ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5726 ConsulRegisterOption:GroupName=UserMicroservice ConsulRegisterOption:HealthCheckUrl=http://localhost:5726/Health ConsulRegisterOption:Tag=13"
::启动品牌微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.Brand.WebApi\DaiMaWuDong.MSACommerce.Brand.WebApi&&dotnet run --urls="http://localhost:5721" --ip="localhost" --port=5721 ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5721 ConsulRegisterOption:GroupName=BrandMicroservice ConsulRegisterOption:HealthCheckUrl=http://localhost:5721/Health ConsulRegisterOption:Tag=13"
::启动类别微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.Category.WebApi\DaiMaWuDong.MSACommerce.Category.WebApi&&dotnet run --urls="http://localhost:5722" --ip="localhost" --port=5722 ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5722 ConsulRegisterOption:GroupName=CategoryMicroservice ConsulRegisterOption:HealthCheckUrl=http://localhost:5722/Health ConsulRegisterOption:Tag=13"
::启动规格微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.Spec.WebApi\DaiMaWuDong.MSACommerce.Spec.WebApi&&dotnet run --urls="http://localhost:5723" --ip="localhost" --port=5723 ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5723 ConsulRegisterOption:GroupName=SpecMicroservice ConsulRegisterOption:HealthCheckUrl=http://localhost:5723/Health ConsulRegisterOption:Tag=13"
::启动商品微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.Goods.WebApi\DaiMaWuDong.MSACommerce.Goods.WebApi&&dotnet run --urls="http://localhost:5633" --ip="localhost" --port=5633 ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5633 ConsulRegisterOption:GroupName=GoodsMicroservice ConsulRegisterOption:HealthCheckUrl=http://localhost:5633/Health ConsulRegisterOption:Tag=13"
::启动商品搜索微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.Search.WebApi\DaiMaWuDong.MSACommerce.Search.WebApi&&dotnet run --urls="http://localhost:5730" ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5730 ConsulRegisterOption:GroupName=GoodsSearchMicroservice ConsulRegisterOption:HealthCheckUrl=http://localhost:5730/Health ConsulRegisterOption:Tag=13"
::启动商品详情微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.PageDetail\DaiMaWuDong.MSACommerce.PageDetail&&dotnet run --urls="http://localhost:5728" ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5728 ConsulRegisterOption:GroupName=PageDetail ConsulRegisterOption:HealthCheckUrl=http://localhost:5728/Health ConsulRegisterOption:Tag=13"
::启动购物车微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.Core.CartMicroservice\DaiMaWuDong.Core.CartMicroservice&&dotnet run --urls="http://localhost:5733" ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5733 ConsulRegisterOption:GroupName=CartMicroService ConsulRegisterOption:HealthCheckUrl=http://localhost:5733/Health ConsulRegisterOption:Tag=13"
::启动订单微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.Core.OrderMicroservice\DaiMaWuDong.Core.OrderMicroservice&&dotnet run --urls="http://localhost:5729" ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5729 ConsulRegisterOption:GroupName=OrderMicroService ConsulRegisterOption:HealthCheckUrl=http://localhost:5729/Health ConsulRegisterOption:Tag=13"
::启动库存微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.MSACommerce.Stock.WebApi\DaiMaWuDong.MSACommerce.Stock.WebApi&&dotnet run --urls="http://localhost:5724" ConsulClientOption:IP=localhost ConsulClientOption:Port=8500 ConsulRegisterOption:IP=localhost ConsulRegisterOption:Port=5724 ConsulRegisterOption:GroupName=StockMicroService ConsulRegisterOption:HealthCheckUrl=http://localhost:5724/Health ConsulRegisterOption:Tag=13"
::启动支付微服务
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.Core.PayMicroservice\DaiMaWuDong.Core.PayMicroservice&&dotnet run --urls="http://192.168.3.126:80" ConsulClientOption:IP=192.168.3.126 ConsulClientOption:Port=8500 ConsulRegisterOption:IP=192.168.3.126 ConsulRegisterOption:Port=80 ConsulRegisterOption:GroupName=PayMicroService ConsulRegisterOption:HealthCheckUrl=http://192.168.3.126:80/Health ConsulRegisterOption:Tag=13"
::启动订单后台处理器
ping 127.0.0.1 -n 6 >nul
start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.Core.OrderMicroservice\DaiMaWuDong.MSACormmerce.OrderProcessor&&dotnet run"
::启动支付后台处理器
::ping 127.0.0.1 -n 6 >nul
::start cmd /k "cd /d E:\daimawudong\代码\DaiMaWuDong.Core.PayMicroservice\DaiMaWuDong.MSACormmerce.PayProcessor&&dotnet run"
::关闭前端nginx
taskkill /f /t /im nginx.exe
::启动前端服务
start cmd /k "cd /d E:\daimawudong\anzhuangbao\nginx-for-portal&&start nginx"
