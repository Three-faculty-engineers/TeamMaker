import React from 'react'
import UserIcon from '../img/icons8-person-30.png'
import { useEffect, useState } from 'react'
function TeamInfo(props) {

  const request = { 
    method: 'GET',
    headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
  }





  return (
    <div className='noDots'>        
        <h2>MEMBERS: </h2>
        <ul className='noDots'>
            {props.clanovi.map((clan) => {return (

              <li key={clan.id}>
              <img className="PanelIcon" 
                  src={props.images.find(image => image.id == clan.id)?.imageBase64 ? props.images.find(image => image.id == clan.id).imageBase64 : UserIcon}/> 
                  {clan.username}
                  
        
            </li>) 
          })}
        </ul>


        <p style={{margin: "20px", padding: "10px", background:"#fafafa"}}>
          {props.teamOpis}
        </p>
    </div>
  )
}

export default TeamInfo