# 

配置下js/common.js    修改下全局vue请求的base-url地址：

axios.defaults.baseURL = "http://localhost:6299/api";



指定跨域的时候不需要credentials，避免后台指定，不需要cookie跨域了

axios.defaults.withCredentials = false;



门户系统面向的是用户，安全性很重要，而且搜索引擎对于单页应用并不友好。因此门户系统不再采用与后台系统类似的SPA（单页应用）。依然是前后端分离，不过前端的页面会使用独立的html，在每个页面中使用vue来做页面渲染。采用live-server热部署方式

~~~text
npm install -g live-server
live-server
~~~



Nginx静态化部署，修改端口，修改配置文件如下

~~~text
        location / {
            root   D:/Project/ArchitectBBS/trunk/Zhaoxi.MSACommerce/Zhaoxi.MSACommerce.Project/Zhaoxi.MSACormmerce.PortalProject/;
        }
~~~

