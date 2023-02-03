import React from 'react'
import { Link } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { ListGroup, Container, Nav, Jumbotron } from 'react-bootstrap'
import MessageBox from '../components/MessageBox'
import Settings from '../components/Settings'
import 'bootstrap/dist/css/bootstrap.min.css'

import PanelIcon from '../img/icons8-home-48.png'
import AllTeamsIcon from '../img/icons8-job-seeker-48.png'
import MyTeamsIcon from '../img/icons8-management-48.png'
import CreateTeamIcon from '../img/icons8-create-48.png'
import SettingsIcon from '../img/icons8-settings-48.png'
import ChatIcon from '../img/icons8-chat-48.png'
import UserIcon from '../img/icons8-person-30.png'


function PanelPage() {

  const [ime, setIme] = useState('')
  const [showState, setShowState] = useState(0)
  const [objave, setObjave] = useState([])

  useEffect(() => {

    //setIme(props.username)

    const request = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
    }  
    
    fetch('https://localhost:7013/Korisnik/getUserFromToken', request).then(response => {
      if(response.ok)
        response.json().then(user => {
          setIme(user.username)
        })
    })
    

    const request2 = { 
      method: 'GET',
      headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
    }

    fetch('https://localhost:7013/Objava/getObjaveSve', request2).then(response =>{
      if(response.ok)
      response.json().then(posts=>{
        console.log(posts)
        setObjave(posts)
      })
    })
    
 }, [])


  return (
    <div className='PanelDiv'>


      <Nav bg="light" className="navigacija">
        <Nav.Item>
          <Nav.Link className="navigacijaDeo" style={{color:"#000"}} href='/'><center>PANEL <img className="PanelIcon" src={PanelIcon}/></center></Nav.Link>
        </Nav.Item>

        <Nav.Item>
          <Nav.Link className="navigacijaDeo" style={{color:"#000"}} href='/teams/allTeams'><center>ALL TEAMS <img className="PanelIcon" src={AllTeamsIcon}/></center></Nav.Link>
        </Nav.Item>

        <Nav.Item>
          <Nav.Link className="navigacijaDeo" style={{color:"#000"}} href='/teams/ownTeams'><center>MY TEAMS <img className="PanelIcon" src={MyTeamsIcon}/></center></Nav.Link>
        </Nav.Item>

        <Nav.Item>
          <Nav.Link className="navigacijaDeo" style={{color:"#000"}} href='/teams/createTeam'><center>CREATE TEAM <img className="PanelIcon" src={CreateTeamIcon}/></center></Nav.Link>
        </Nav.Item>

        <Nav.Item onClick={() => {setShowState(2)}}>
          <Nav.Link className="navigacijaDeo" style={{color:"#000"}}><center>SETTINGS <img className="PanelIcon" src={SettingsIcon}/></center></Nav.Link>
        </Nav.Item>
        
        <Nav.Item onClick={() => {setShowState(1)}}>
          <Nav.Link className="navigacijaDeo" style={{color:"#000"}}><center>CHAT <img className="PanelIcon" src={ChatIcon}/></center></Nav.Link>
        </Nav.Item>
      </Nav>

      { (showState == 0) && <Container style={{padding: "10px", margin:"10px"}}>

        <h1>Welcome back, {ime}!</h1>

        <div className='notifBox' id='notifs'>

        { (objave.length == 0) && <div className='prazno'><h2>... No posts just yet! :)</h2></div> }


     {objave.map(objava => {
                return (        
                <div  key={objava.id} style={{margin:"20px"}}>
                  <img className="PanelIcon" src={UserIcon}/> {objava.korisnik} in {objava.team}
                    <div className='myMessage'>{objava.poruka}</div>
                    {new Date(objava.vreme).toUTCString()}
                </div>
                )
            })}

                   
      </div>
      </Container>}

      { (showState == 1) && <MessageBox />}
      { (showState == 2) && <Settings username={ime}/> }

    </div>
        
  )
}

export default PanelPage