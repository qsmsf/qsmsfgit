import {
  SET_USERLIST,
  SET_UNITLIST,
  CLR_LIST
} from '../mutationTypes'
import Vue from 'vue'

const state = {
  // 保存第一页数据
  userList: [[{name: '', value: ''}]],
  unitList: [[{name: '', value: ''}]],
  userNameList: [],
  unitNameList: []
}

const mutations = {
  // 设置 token 登录名 头像
  [SET_USERLIST] (state, data) {
    try {
      let userList = [[{name: '无', value: '0', id: 0}]]
      let userNameList = [[]]
      for (var i = 0; i < data.length; i++) {
        let tmp = {
          name: data[i].user_fullname,
          value: data[i].user_id + '',
          id: data[i].user_id
        }
        userList[0].push(tmp)
        userNameList[0].push(data[i].user_fullname)
      }
      state.userList = userList
      state.userNameList = userNameList
    } catch (err) {
      console.log(err)
    }
  },
  [SET_UNITLIST] (state, data) {
    try {
      let unitList = [[{name: '无', value: '0', id: 0}]]
      let unitNameList = [[]]
      for (var i = 0; i < data.length; i++) {
        let tmp = {
          name: data[i].unit_name,
          value: data[i].unit_id + '',
          id: data[i].unit_id
        }
        unitList[0].push(tmp)
        unitNameList[0].push(data[i].unit_name)
      }
      state.unitList = unitList
      state.unitNameList = unitNameList
    } catch (err) {
      console.log(err)
    }
  },
  [CLR_LIST] (state) {
    state.userList = [[{name: '', value: ''}]]
    state.unitList = [[{name: '', value: ''}]]
    state.unitNameList = [[]]
    state.userNameList = [[]]
  }
}

const actions = {
  'fetchUserList': function (store, params) {
    Vue.http({
      url: params.baseUrl + 'api/Users/GetUserList',
      method: 'Get',
      emulateJSON: true,
      params: {
        myUnitId: params.myUnitId
      },
      headers: {
        contentType: 'application/x-www-form-urlencoded',
        token: params.token
      }
    }).then(function (res) {
      // console.log(res.data)
      // let data = JSON.parse(res.data)
      if (res.data.code === 100) {
        store.commit(SET_USERLIST, res.data.resultList)
      } else {
        console.log(res.data.message)
      }
    }).catch(err => {
      console.log(err)
    })
  },
  'fetchUnitList': function (store, params) {
    Vue.http({
      url: params.baseUrl + 'api/Unit/GetUnitList',
      method: 'Get',
      emulateJSON: true,
      params: {
        myUnitId: params.myUnitId
      },
      headers: {
        contentType: 'application/x-www-form-urlencoded',
        token: params.token
      }
    }).then(function (res) {
      // let data = JSON.parse(res.data)
      if (res.data.code === 100) {
        store.commit(SET_UNITLIST, res.data.resultList)
      } else {
        console.log(res.data.message)
      }
    }).catch(err => {
      console.log(err)
    })
  },
  'clearList': function (store) {
    store.commit(CLR_LIST)
  }
}
const getters = {
  GetterUnitState: state => {
    return state.unitList
  },
  GetterUserState: state => {
    return state.userList
  },
  GetterUnitNameState: state => {
    return state.unitNameList
  },
  GetterUserNameState: state => {
    return state.userNameList
  }
}
export default {
  state,
  mutations,
  actions,
  getters
}
