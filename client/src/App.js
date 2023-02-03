import React from 'react'
import './App.css'
import { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom';
import { Link } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import Navigacija from './components/Navigacija';
import PanelPage from './pages/PanelPage';
import AllTeamsPage from './pages/AllTeamsPage';
import CreateTeamPage from './pages/CreateTeamPage';
import SingleTeamPage from './pages/SingleTeamPage';
import OwnTeamsPage from './pages/OwnTeamsPage';
import About from './pages/About';
import AdminPage from './pages/AdminPage.js'
import RegisterPage from './pages/RegisterPage';
import RequireAuth from './components/RequireAuth';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {

  const [user, setUser] = useState({"username": "", "password":""})
  const [token, setToken] = useState('')//verovatno treba koristiti kao neki globalni context
  const [isLogged, setIsLogged] = useState(false)
  const [showNav, setShowNav] = useState(true)
  const [username, setUsername] = useState('')

  const ROLES = 
  {
    'USER' : 1,
    'ADMIN' : 2
  }

  const navigate = useNavigate();

  const onLogin = (token, usernm, obj) =>
  {    
    
    sessionStorage.setItem("jwt", token)
    sessionStorage.setItem("username", usernm)
    sessionStorage.setItem("auth", JSON.stringify(obj))

    setToken(token)
    setUsername(usernm)           
    setIsLogged(true)

  }

  const logout = () =>
  {

    sessionStorage.removeItem("jwt")
    sessionStorage.removeItem("username")
    sessionStorage.removeItem("auth")

    setToken("")
    setUsername("")
    setIsLogged(false)

    navigate('/login')

  }
  // useEffect(() => {
    
  //   console.log(username)
  //   return () => {console.log("dismount")}
  // }, [username])
  
  useEffect(() => {
    if(sessionStorage.getItem("jwt") != null)
      {
        setToken(sessionStorage.getItem("jwt"))
        setUsername(sessionStorage.getItem("username"))
        setIsLogged(true)
      }

  }, [isLogged])

  return (

      <div className='Main'> 
        {showNav ? <Navigacija isLogged = {isLogged} logout = {logout}/> : null}
        <Routes>
          <Route path='/about' element={<About />} />
          <Route path='/login' element={<LoginPage onLogin={onLogin} />} />
          <Route path='/RegisterPage' element={ <RegisterPage/> } />

          <Route element={<RequireAuth allowedRole = {ROLES.USER}/>}>
            
            <Route path='/' element={ <PanelPage /> } />
            <Route path='/teams/allTeams' element={<AllTeamsPage /> } />
            <Route path='/teams/singleteam/:teamid' element={ <SingleTeamPage /> } />
            <Route path='/teams/createTeam'  element={ <CreateTeamPage/> } />
            <Route path='/teams/ownTeams' element={ <OwnTeamsPage/> } />
          </Route> 
          
          <Route element={<RequireAuth allowedRole = {ROLES.ADMIN}/>}>
            <Route path='/AdminPage' element={<AdminPage />}/>
          </Route>

        </Routes>
      </div>


  )
}

export default App;

/*
//import { BrowserRouter as Router, Routes, Route}
//    from 'react-router-dom';
//import Navbar from './components/Navbar';
    <Router>
    <Navbar />
    <Routes>
        <Route exact path='/'  element={<div></div>} />
        <Route path='/login' element={<LoginForm onLogin={onLogin}/>} />
  
    </Routes>
    </Router>

*/