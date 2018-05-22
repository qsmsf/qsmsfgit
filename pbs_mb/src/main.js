// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import FastClick from 'fastclick'
import VueRouter from 'vue-router'
import App from './App'
import routeConfig from './router/index'
import store from './store'
import VueResource from 'vue-resource'
import BaiduMap from 'vue-baidu-map'

Vue.use(VueResource)
Vue.use(VueRouter)
Vue.use(BaiduMap, {
  ak: 'Z7g5hqhlkpShrdb4ph4znpdGAj2OPE3w'
})

const router = new VueRouter({
  routes: routeConfig
})

import { LocalePlugin, DevicePlugin, ToastPlugin, AlertPlugin, ConfirmPlugin, LoadingPlugin, WechatPlugin } from 'vux'
Vue.use(DevicePlugin)
Vue.use(ToastPlugin)
Vue.use(AlertPlugin)
Vue.use(ConfirmPlugin)
Vue.use(LoadingPlugin)
Vue.use(WechatPlugin)
Vue.use(LocalePlugin)

FastClick.attach(document.body)

/* eslint-disable no-new */
document.addEventListener('deviceready', function() {

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app-box')
window.navigator.splashscreen.hide()
}, false);