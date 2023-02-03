import React from 'react'
import { useState, useEffect } from 'react'
import { Link, useParams } from 'react-router-dom'//za izvlacenje paramsa iz route-a
import ReqJoinBtn from './ReqJoinBtn'
import { Card, Button, Container } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'
import MyTeamsIcon from '../img/icons8-management-48.png'
import UserIcon from '../img/icons8-person-30.png'


function Team({tm, isOwn}) {
    //tm = team :/
    const { username } = useParams()
    const [korisnici, setKorisnici] = useState([])
    const [images, setImages] = useState([])

    useEffect(() =>{

        setKorisnici(tm.korisnici)
        
        const request = { 
            method: 'GET',
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
        }

        //if(images.length == 0)
        fetch('https://localhost:7013/Korisnik/getTeamPictures/' + tm.id, request).then((response) => {
          if(response.ok)
            response.json().then((images) => {
              setImages(images)
            })
        })



    },[])

    return (
        <Card className='Team'>
            <Card.Header className='naslov'><Link to={`/teams/singleteam/${tm.id}`} state={tm} ><h3> <img className="searchIcon" src={MyTeamsIcon}/> {tm.ime}</h3> </Link></Card.Header>
            
            <div style={{padding:"20px", margin:"5px"}}>

                <ul>
                    {korisnici.map((kor)=>(
                    
                    <li key={kor.id}><img className="PanelIcon" src={images.find(image => image.id == kor.id)?.imageBase64 ? images.find(image => image.id == kor.id).imageBase64 : UserIcon}/> {kor.username}</li>
                    
                    ))}

                </ul>

                <p>{tm.opis}</p>

            </div>
            
            <center>{!isOwn ? <ReqJoinBtn teamid = {tm.id} /> : null }</center>

        </Card>
    )

}

export default Team