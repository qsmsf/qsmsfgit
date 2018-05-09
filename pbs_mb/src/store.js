import Vue from 'vue'
import Vuex from 'vuex'
import userInfo from 'modules/userInfo'
import dropDownList from 'modules/dropDownList'
import recordEntity from 'modules/recordEntity'
import sysInfo from 'modules/sysInfo'

const debug = process.env.NODE_ENV !== 'production'
Vue.use(Vuex)
Vue.config.debug = debug

export default new Vuex.Store({
  modules: {
    userInfo,
    dropDownList,
    recordEntity,
    sysInfo
  },
  strict: debug
})
