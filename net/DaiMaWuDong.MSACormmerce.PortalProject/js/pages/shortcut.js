const shortcut = {
    template: "\
    <div class='py-container'> \
        <div class='shortcut'> \
            <ul class='fl'> \
               <li class='f-item'>易淘欢迎您！</li> \
               <li class='f-item' v-if='user && user.username'>\
               尊敬的会员，<span style='color: red;'>{{user.username}}</span>\
               </li>\
               <li v-else class='f-item'> \
                   请<a href='javascript:void(0)' @click='gotoLogin'>登录</a>　 \
                   <span><a href='/register.html' target='_blank'>免费注册</a></span> \
               </li> \
           </ul> \
           <ul class='fr'> \
               <li class='f-item'>我的订单</li> \
               <li class='f-item space'></li> \
               <li class='f-item'><a href='/home-index.html' target='_blank'>我的易淘</a></li> \
               <li class='f-item space'></li> \
               <li class='f-item'>易淘会员</li> \
               <li class='f-item space'></li> \
               <li class='f-item'>企业采购</li> \
               <li class='f-item space'></li> \
               <li class='f-item'>关注易淘</li> \
               <li class='f-item space'></li> \
               <li class='f-item' id='service'> \
                   <span>客户服务</span> \
                   <ul class='service'> \
                       <li><a href='/cooperation.html' target='_blank'>合作招商</a></li> \
                       <li><a href='/shoplogin.html' target='_blank'>商家后台</a></li> \
                       <li><a href='/cooperation.html' target='_blank'>合作招商</a></li> \
                       <li><a href='#'>商家后台</a></li> \
                   </ul> \
               </li> \
               <li class='f-item space'></li> \
               <li class='f-item'>网站导航</li> \
           </ul> \
       </div> \
    </div>\
    ",
    name: "shortcut",
    data() {
        return {
            user: null
        }
    },
    created() {
        yt.http.get("/user/verify", {
            headers: {
                "Authorization": "Bearer " + localStorage["token"]
            }
        }).then(resp => {
            if (resp.data.result) {
                this.user = resp.data.value;
            }
            else {
                console.log("暂未登陆");
            }
        });
        //yt.http.get("/user/getwithauthorize", {
        //    headers: {
        //        "Authorization": "Bearer " + localStorage["token"]
        //    }
        //}).then(resp => {
        //    if (resp.data.result) {
        //        alert(resp.data.message);
        //    }
        //    else {
        //        console.log("暂未登陆");
        //    }
        //})
    },
    //created() {
    //    yt.http.get("/user/verify", {
    //        headers: {
    //            "Authorization": "Bearer " + localStorage["token"]
    //        }
    //    }).then(resp => {
    //        if (resp.data.result) {
    //            this.user = resp.data.value;
    //        }
    //        else if (resp.data.statusCode == 401)//过期
    //        {
    //            let refreshToken = localStorage["refreshToken"];
    //            if (refreshToken != null) {
    //                yt.http.post("/auth/refresh", {
    //                    "refreshToken": localStorage["refreshToken"]
    //                })
    //                    .then(resp => {
    //                        if (resp.data.result) {
    //                            localStorage["token"] = resp.data.value;
    //                            this.created();
    //                        }
    //                        else {
    //                            console.log("刷新token已过期");
    //                        }
    //                    })
    //            }
    //        }
    //        else {
    //            console.log("暂未登陆");
    //        }
    //    })
    //},
    methods: {
        gotoLogin() {
            window.location = "/login.html?returnUrl=" + window.location;
        }
    }
}
export default shortcut;
