import React from 'react'
import { useState, useEffect } from 'react'
import { Button } from 'react-bootstrap'
import UserIcon from '../img/icons8-person-30.png'
import LawIcon from '../img/icons8-law-30.png'


function TeamLeader(props) {
  
  const [requests, setRequests] = useState([])
  const [lookingForMembers, setLookingForMembers] = useState(props.team.needsMembers)
  const request = {
    method: 'GET',
    headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
  } 
  const request2 = {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
} 
  useEffect(() => {
    fetch('https://localhost:7013/Invite/GetRequestsForTeam/'+props.teamID, request).then( (response) => {
      if(response.ok){
          response.json().then((requests) => {
            setRequests(requests)
          })
      }
    })


  }, [lookingForMembers])

  const acceptRequest = (id, username) => {
    fetch(`https://localhost:7013/Invite/AcceptUser/${username}/${id}/${props.teamID}`, request2).then( (response) => {
      if(response.ok){
          response.json().then((res) => {

            props.setClanovi([...props.clanovi, {id:res.id, username: res.username}])
            setRequests(requests.filter((request) => request.id != id))

          })
      }
    })
  }
  const kickUser = (userID) => {
    if(window.confirm("are yoy sure\nnovired"))
    {
      fetch(`https://localhost:7013/Team/KickUserFromTeam/${props.teamID}/${userID}`, request2).then( (response) => {
        if(response.ok){
          props.setClanovi(props.clanovi.filter((clan) => clan.id != userID))
        }
      })
    }
    else
      return
  
  }
  

  const updateTeam = (flag) => {
    fetch(`https://localhost:7013/Team/lookingForMembers/${props.teamID}/${flag}`, request2).then( (response) => {
        if(response.ok){
          setLookingForMembers(flag)
          props.team.needsMembers = flag
          props.setTeam(props.team)
        }
      })
  }


  return (
    <div>

      <h2>Requests to join team</h2>
      <div className='sprintlist'>
      {requests.map((request) => (
      <div key={request.id} className='sprint'>
        <p><img className="PanelIcon" src={UserIcon}/> User:</p><p>{request.username}</p> 
        <p>Message:</p> <p>{request.poruka}</p> 
        <Button variant="dark" onClick={() => {acceptRequest(request.id, request.username)}}>AcceptUser</Button> 
      </div>))}
      </div>
      

      <h2>Members</h2>
      <div className='sprintlist'>
      {props.clanovi.map((clan) => (
        <div key={clan.id} className='sprint'>
          <p><img className="PanelIcon" src={UserIcon}/> User: {clan.username}</p> 
          <Button variant="dark" onClick={() => {kickUser(clan.id)}}>
          <img className="PanelIcon" src={LawIcon}/> Kick User</Button> 

        </div> 
        ))
      }
      </div>

      <h2>Looking for new members</h2>
      <Button variant="dark" onClick={() => {updateTeam(lookingForMembers ? false : true)}}>
        {lookingForMembers ? "NE TRAZI" : "TRAZI"}
      </Button> 
    </div>
  )
  
}

export default TeamLeader