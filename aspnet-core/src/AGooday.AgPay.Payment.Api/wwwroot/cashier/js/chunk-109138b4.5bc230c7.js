(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-109138b4"],{"0720":function(t,e,a){t.exports=a.p+"img/zfb.f9f04ed3.jpeg"},"4cd6":function(t,e,a){"use strict";var o=function(){var t=this,e=t._self._c;return e("div",{staticClass:"keyboard"},[e("div",{staticClass:"keyboard-top"},[e("div",{staticClass:"triangle-topleft-k"}),e("div",{staticClass:"keyboard-tite"},[t._v("@Ag支付")]),e("div",{staticClass:"triangle-topleft-k",on:{click:t.concealSateFn}},[e("div",{staticClass:"triangle-topleft",style:t.concealSate?"":"transform:rotate(-135deg);margin-top: 12px;"})])]),e("div",{directives:[{name:"show",rawName:"v-show",value:t.concealSate,expression:"concealSate"}],staticClass:"keyboard-main",staticStyle:{transition:"all 1s ease"}},[t._l(t.numberList,(function(o,r){return e("div",{key:r,staticClass:"keyborad-key"},t._l(o,(function(o,r){return e("div",{key:r,ref:"number",refInFor:!0,staticClass:"number",on:{touchstart:function(e){return e.preventDefault(),t.goTouchstart(o,e)},touchend:function(e){return e.preventDefault(),t.goTouchend(o,e)}}},[t._v(" "+t._s("del"!=o?o:"")+" "),"del"==o?[e("img",{attrs:{src:a("f809"),alt:""}})]:t._e()],2)})),0)})),e("div",{staticClass:"keyborad-key"},[e("div",{staticClass:"number zero",on:{touchstart:function(e){return e.preventDefault(),t.goTouchstart("zero",e)},touchend:function(e){return e.preventDefault(),t.goTouchend("zero",e)}}},[t._v(" 0 ")]),e("div",{staticClass:"number",on:{touchstart:function(e){return e.preventDefault(),t.goTouchstart("dot",e)},touchend:function(e){return e.preventDefault(),t.goTouchend("dot",e)}}},[e("div",{staticClass:"dot"})])])],2),e("div",{class:t.paymentClassFn,style:"background:"+t.typeColor+";",on:{touchstart:function(e){return e.preventDefault(),t.goTouchstart("pay",e)},touchend:function(e){return e.preventDefault(),t.goTouchend("pay",e)}}},[e("div",{staticClass:"pay"},[t._v("付款")])])])},r=[],n={name:"Keyboard",data(){return{timeOutEvent:0,tiemIntervalEvent:0,concealSateC:!0,numberList:[[1,2,3,"del"],[4,5,6,"C"],[7,8,9]]}},computed:{paymentClassFn(){let t=this.concealSate?"payment":"paymentConceal",e=-1!=this.money&&""!=this.money?"":"opacityS";return t+" "+e}},props:{typeColor:{type:String,default:"#07c160"},touchTypeColor:{type:String,default:"rgba(7, 130, 65, 1)"},money:{type:String|Number,default:-1},concealSate:{type:Boolean,default:!0}},mounted(){this.concealSateC=this.concealSate},methods:{concealSateFn(){this.$emit("conceal")},onKeyboard(t,e){if(setTimeout(()=>{e.style.background="pay"==t?this.typeColor:"#fff"},100),"del"==t)return void this.$emit("delTheAmount",t);if("C"==t)return void this.$emit("clearTheAmount",t);if("pay"==t)return void this.$emit("payment");let a={zero:0,dot:"."};"number"!=typeof t&&(t=a[t]),this.$emit("enterTheAmount",t)},goTouchstart(t,e){if(e="img"==e.target.localName||"dot"==e.target.className||"pay"==e.target.className?e.target.parentNode:e.target,"pay"==t){if(-1==this.money||""==this.money)return;e.style.background=this.touchTypeColor}else e.style.background="rgba(197, 197, 197, 0.7)";let a=this;clearTimeout(a.timeOutEvent),a.timeOutEvent=setTimeout((function(){if(a.timeOutEvent=0,"del"==t)return clearInterval(a.tiemIntervalEvent),void a.delLong(t)}),600)},goTouchend(t,e){if(e="img"==e.target.localName||"dot"==e.target.className||"pay"==e.target.className?e.target.parentNode:e.target,"pay"==t){if(-1==this.money||""==this.money)return;e.style.background=this.typeColor}else e.style.background="#fff";let a=this;clearTimeout(a.timeOutEvent),clearInterval(a.tiemIntervalEvent),0!==a.timeOutEvent&&this.onKeyboard(t,e)},delLong(t){this.tiemIntervalEvent=setInterval(()=>{this.$emit("delTheAmount",t)},200)}}},i=n,s=(a("c538"),a("2877")),l=Object(s["a"])(i,o,r,!1,null,"587f609d",null);e["a"]=l.exports},9454:function(t,e,a){},"950a":function(t,e,a){"use strict";a("bae0")},"99ea":function(t,e,a){"use strict";var o=function(){var t=this,e=t._self._c;return e("div",{staticClass:"dialog"},[e("div",{staticClass:"dialog-box"},[e("div",{staticClass:"dialog-remark",style:"color:"+t.typeColor+";"},[t._v("添加备注")]),e("div",{staticClass:"dialog-input"},[e("input",{directives:[{name:"model",rawName:"v-model",value:t.remarkC2,expression:"remarkC2"}],attrs:{type:"text",placeholder:"最多输入12个字",maxlength:"12"},domProps:{value:t.remarkC2},on:{input:function(e){e.target.composing||(t.remarkC2=e.target.value)}}})]),e("div",{staticClass:"dialog-bnt"},[e("div",{staticClass:"dialog-bnt-l",on:{click:function(e){return t.myDialogStateFn(!1)}}},[t._v("取消")]),e("div",{staticClass:"dialog-bnt-r",style:"background:"+t.typeColor+";border-top-color:"+t.typeColor+";",on:{click:function(e){return t.myDialogStateFn(!0)}}},[t._v("确认")])])])])},r=[],n={name:"MyDialog",data(){return{remarkC:"",remarkC2:""}},props:{typeColor:{type:String,default:"#07c160"},remark:{type:String,default:()=>""}},created(){this.remarkC=this.remark,this.remarkC2=this.remark},methods:{myDialogStateFn(t){let e=this.remarkC2,a=this.remarkC;this.$emit("myDialogStateFn",t?e+"":a+""),!t&&(this.remarkC2=a),t&&(this.remarkC=e)}}},i=n,s=(a("950a"),a("2877")),l=Object(s["a"])(i,o,r,!1,null,"17344372",null);e["a"]=l.exports},a3c3:function(t,e,a){},bae0:function(t,e,a){},c538:function(t,e,a){"use strict";a("a3c3")},d025:function(t,e,a){"use strict";a("9454")},d5eb:function(t,e,a){"use strict";a.r(e);var o=function(){var t=this,e=t._self._c;return e("div",{staticClass:"pay-panel"},[e("div",{staticClass:"content"},[e("div",{staticClass:"content-top-bg",style:"background:"+t.typeColor[t.payType]+";"}),e("div",{staticClass:"content-body"},[e("header",{staticClass:"header"},[e("div",{staticClass:"header-text"},[t._v("付款给 "+t._s(t.merchantName))]),e("div",{staticClass:"header-img"},[e("img",{attrs:{src:t.avatar?t.avatar:t.icon_member_default,alt:""}})])]),e("div",{staticClass:"plus-input"},[e("div",{staticClass:"S"},[e("span",{style:"color:"+t.typeColor[t.payType]+";"},[t._v("￥")])]),e("div",{staticClass:"input-c",style:"color:"+t.typeColor[t.payType]+";"},[e("div",{staticClass:"input-c-div-1"},[t._v(t._s(t.formatMoney(t.amount)))]),e("div",{staticClass:"input-c-div",style:"background:"+t.typeColor[t.payType]+";border-color:"+t.typeColor[t.payType]+";"})]),e("div",{directives:[{name:"show",rawName:"v-show",value:!t.amount,expression:"!amount"}],staticClass:"placeholder"},[t._v("请输入金额")])])])]),e("ul",{staticClass:"plus-ul"},[e("li",{staticStyle:{"border-radius":"10px"}},[e("div",{staticClass:"img-div"},[e("img",{attrs:{src:t.payImg,alt:""}}),e("div",{staticClass:"div-text"},[t._v(" 支付宝支付 ")])])])]),e("div",{staticClass:"remark-k",class:"wx"!=t.payType?"margin-top-30":""},[e("div",{staticClass:"remark"},[e("div",{directives:[{name:"show",rawName:"v-show",value:t.remark,expression:"remark"}],staticClass:"remark-hui"},[t._v(t._s(t.remark))]),e("div",{style:"color:"+t.typeColor[t.payType]+";",on:{click:function(e){return t.myDialogStateFn(t.remark)}}},[t._v(t._s(t.remark?"修改":"添加备注"))])])]),e("MyDialog",{directives:[{name:"show",rawName:"v-show",value:t.myDialogState,expression:"myDialogState"}],attrs:{remark:t.remark,typeColor:t.typeColor[t.payType]},on:{myDialogStateFn:t.myDialogStateFn}}),t.isAllowModifyAmount?e("div",{staticClass:"keyboard-plus"},[e("Keyboard",{attrs:{money:t.money,concealSate:t.concealSate,typeColor:t.typeColor[t.payType],touchTypeColor:t.touchTypeColor[t.payType]},on:{delTheAmount:t.delTheAmount,conceal:t.conceal,enterTheAmount:t.enterTheAmount,clearTheAmount:t.clearTheAmount,payment:t.payment}})],1):t._e(),t.isAllowModifyAmount?t._e():e("div",{staticClass:"bnt-pay"},[e("div",{staticClass:"bnt-pay"},[e("div",{staticClass:"bnt-pay-text",style:"background:"+t.typeColor[t.payType]+";",on:{click:t.pay}},[t._v(" 付款 ")])])])],1)},r=[],n=(a("14d9"),a("4ec3")),i=a("f121"),s=a("99ea"),l=a("4cd6"),c={components:{MyDialog:s["a"],Keyboard:l["a"]},data:function(){return{merchantName:"agpay",avatar:a("0720"),amount:"",resData:{},payImg:a("0720"),payOrderInfo:{},isAllowModifyAmount:!0,myDialogState:!1,remark:"",money:-1,concealSate:!0,payType:"alipay",typeColor:{alipay:"#1678ff",wxpay:"#07c160",ysfpay:"#ff534d"},touchTypeColor:{alipay:"rgba(20, 98, 206, 1)",wxpay:"rgba(7, 130, 65, 1)",ysfpay:"rgb(248 70 65, 1)"}}},mounted(){},methods:{payment(){-1!=this.money&&console.log("payment")},conceal(){this.concealSate=!this.concealSate},enterTheAmount(t){this.amount>=9999999||"."===t&&this.amount.includes(".")||this.amount.includes(".")&&this.amount.split(".").pop().length>=2||("0"===this.amount?this.amount=t.toString():this.amount=`${this.amount}${t}`,"."===this.amount&&(this.amount="0."),Number.isNaN(this.amount)?this.payOrderInfo.amount=0:this.payOrderInfo.amount=100*this.amount,this.money=this.payOrderInfo.amount>0?this.payOrderInfo.amount:-1)},delTheAmount(){this.amount=this.amount.substring(0,this.amount.length-1),Number.isNaN(this.amount)?this.payOrderInfo.amount=0:this.payOrderInfo.amount=100*this.amount,this.money=this.payOrderInfo.amount>0?this.payOrderInfo.amount:-1},clearTheAmount(){this.amount="",this.payOrderInfo.amount=0,this.money=-1},formatMoney(t){let e=t.toString().split("."),a=e[0].replace(/(\d)(?=(?:\d{3})+$)/g,"$1,"),o=e.length>1?"."+e.pop():"";return`${a}${o}`},myDialogStateFn:function(t){this.remark=t,this.myDialogState=!this.myDialogState},setPayOrderInfo(t){const e=this;Object(n["b"])().then(a=>{e.payOrderInfo=a,e.merchantName=a.mchName,e.amount=a.amount,t&&e.pay()}).catch(t=>{e.$router.push({name:i["a"].errorPageRouteName,params:{errInfo:t.msg}})})},pay:function(){let t=this;Object(n["c"])(this.amount).then(e=>"0"!=e.code?alert(e.msg):1!=e.data.orderState?alert(e.data.errMsg):void(window.AlipayJSBridge?t.doAlipay(e.data.alipayTradeNo):document.addEventListener("AlipayJSBridgeReady",(function(){t.doAlipay(e.data.alipayTradeNo)}),!1))).catch(e=>{t.$router.push({name:i["a"].errorPageRouteName,params:{errInfo:e.msg}})})},doAlipay(t){const e=this;AlipayJSBridge.call("tradePay",{tradeNO:t},(function(t){"9000"==t.resultCode?e.payOrderInfo.returnUrl?location.href=e.payOrderInfo.returnUrl:(alert("支付成功！"),window.AlipayJSBridge.call("closeWebview")):"8000"==t.resultCode||"6004"==t.resultCode?(alert(JSON.stringify(t)),window.AlipayJSBridge.call("closeWebview")):(alert("用户已取消！"),window.AlipayJSBridge.call("closeWebview"))}))}}},u=c,d=(a("d025"),a("2877")),m=Object(d["a"])(u,o,r,!1,null,"8ffa0bc8",null);e["default"]=m.exports},f809:function(t,e,a){t.exports=a.p+"img/del.1b9149d6.svg"}}]);
//# sourceMappingURL=chunk-109138b4.5bc230c7.js.map