
//全局变量，端口是否转发
isProxy = false;
var debug = false;
var test =false;

//端口映射地址
var url = "http://172.28.0.56:9000";
url = app.getUrl(url);

//服务器真实地址
var _url = "http://68.68.18.242:8010/webapi";
//全局变量，app名称和版本信息
APP_NAME = "PBS";

APP_VERSION = "v1.00";

isUsingNativeGetAndPost = false;

SERVER_CHARSET = "GBK";
