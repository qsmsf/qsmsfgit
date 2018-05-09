import {
  SET_SYSINFO
} from '../mutationTypes'

const state = {
  // 保存第一页数据
  baseUrl: ''
}

const mutations = {
  // 设置 token 登录名 头像
  [SET_SYSINFO] (state, data) {
    try {
      state.baseUrl = data
    } catch (err) {
      console.log(err)
    }
  }
}

const actions = {
}

const getters = {
  GetterBaseUrl: state => {
    return state.baseUrl
  },
  GetterSysInfo: state => {
    return state
  }
}

export default {
  state,
  mutations,
  actions,
  getters
}
