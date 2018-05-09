// content
import {
  SET_BASEINFO,
  CLR_BASEINFO
} from '../mutationTypes'

const state = {
  // 保存第一页数据
  code: '',
  token: '',
  member: null,
  message: ''
}

const mutations = {
  // 设置 token 登录名 头像
  [SET_BASEINFO] (state, data) {
    try {
      state.code = data.code
      state.member = data.member
      state.message = data.message
      state.token = data.token
    } catch (err) {
      console.log(err)
    }
  },
  [CLR_BASEINFO] (state) {
    state.code = ''
    state.member = null
    state.message = ''
    state.token = ''
  }
}
const actions = {
}

const getters = {
  GetterToken: state => {
    return state.token
  },
  GetterMe: state => {
    return state
  }
}

export default {
  state,
  mutations,
  actions,
  getters
}
