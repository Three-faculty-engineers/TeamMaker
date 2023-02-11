import React from 'react'
import { useState, useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import Backlog from '../components/Backlog'
import SprintList from '../components/SprintList'
import { Card } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'
import TeamInfo from '../components/TeamInfo'
import Notifications from '../components/Notifications'
import ChartList from '../components/ChartList'
import TeamLeader from '../components/TeamLeader'
import MyTeamsIcon from '../img/icons8-management-48.png'

function SingleTeamPage() 
{
  const navigate = useNavigate()
  const location = useLocation()

  const [clanovi, setClanovi] = useState([])
  const [isLeader, setIsLeader] = useState(false)
  const [sidebarState, setSidebarState] = useState(0)
  const [team, setTeam] = useState([])

  const [teamID, setTeamID] = useState()
  const [images, setImages] = useState([])


  useEffect(() =>{

    const request = { 
      method: 'GET',
      headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
    }

    //fetch-ujemo podatke za tim dva puta, jednom u ownteamspage.js ili allteamspage.js, i drugi put ovde, mozda nije dobroxd
    fetch('https://localhost:7013/Team/GetTeam/'+ location.state.id, request).then( response => {
        
      if(response.ok)
      {
        response.json().then(team => {
          let usernameGlobal = sessionStorage.getItem("username")
          //provera da li je trenutni korisnik, leader ovog tima
          if(team.leader[0].username.trim() === usernameGlobal.trim()){
              setIsLeader(true)
          }
          setTeam(team)
          setClanovi(team.korisnici)
        })
      } 
      else
      {
        navigate('/')
      }

    })


    fetch('https://localhost:7013/Korisnik/getTeamPictures/' + location.state.id, request).then((response) => {
      if(response.ok)
        response.json().then((images) => {
          setImages(images)
        })
    })

  },[])
  
  const changeSidebar = (x) => {
    setSidebarState(x)//dupla funkcija visak
  }
//proverti za sve komponente u projektu da li koriste sve propse koji su im prosledjeni
  return (
    <div className='singleTeam'>
      
      <div>
        <h1>{team.ime}</h1>

        <ul className='sidebar container'>
          <li className='obojenoHover' onClick={() => {changeSidebar(0)}}>TEAM INFO</li>
          <li className='obojenoHover' onClick={() => {changeSidebar(1)}}>BACKLOG</li>
          <li className='obojenoHover' onClick={() => {changeSidebar(2)}}>ACTIVE SPRINTS</li>
          {/* <li className='obojenoHover' onClick={() => {changeSidebar(3)}}>FINISHED SPRINTS</li> */}
          {/* <li className='obojenoHover' onClick={() => {changeSidebar(4)}}>CHARTS</li> */}
          <li className='obojenoHover' onClick={() => {changeSidebar(5)}}>NOTIFICATIONS</li>
          {isLeader && <li className='obojenoHover' onClick={() => {changeSidebar(6)}}>LEADER PAGE</li>}

          </ul>

        <div>
        { (sidebarState == 0) && <TeamInfo teamID = {team.id} teamOpis={team.opis} isLeader = {isLeader} clanovi = {clanovi} images = {images}  />}
        { (sidebarState == 1) && <Backlog teamID = {team.id} setSidebarState = {setSidebarState} /> }
        { (sidebarState == 2) && <SprintList teamID = {team.id} isLeader = {isLeader} status = {0}/> }
        {/* { (sidebarState == 3) && <SprintList teamID = {team.id} isLeader = {isLeader} status = {1}/> } */}
        {/* { (sidebarState == 4) && <ChartList teamID = {team.id} isLeader = {isLeader} /> } */}
        { (sidebarState == 5) && <Notifications teamID = {team.id} /> }
        { (sidebarState == 6) && <TeamLeader teamID = {team.id} setClanovi = {setClanovi} clanovi = {clanovi} team = {team} setTeam = {setTeam}/> }
        </div>

        </div>

    </div>
  )

}

export default SingleTeamPage