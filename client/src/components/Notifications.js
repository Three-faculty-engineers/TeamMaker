import React from 'react'
import { useState, useEffect } from 'react'

import { Button, Form } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'
import SendIcon from '../img/icons8-email-send-30.png'
import UserIcon from '../img/icons8-person-30.png'

function Notifications(props) {

    const [objave, setObjave] = useState([])
    const [poruka, setPoruku] = useState("")

    const onSubmit = (event) => {
        event.preventDefault()

        if(poruka == "")
          return
    
        const request = {
          method: 'POST',
          headers: {'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
        } 
    
        fetch(`https://localhost:7013/Objava/CreateObjava/${props.teamID}/${poruka}`, request).then(response => {
            if(response.ok)
            {
                alert("notification posted")
                setPoruku("")
                setObjave([])
            }
        })

    }


    useEffect(() => {


        const request = { 
            method: 'GET',
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
        }
        
        if(objave.length == 0)
            fetch('https://localhost:7013/Objava/GetObjave/' + props.teamID, request).then( response => {
                if(response.ok)
                    response.json().then(objave => {
                        setObjave(objave)
                    })
            })
    
    }, [objave])


    return (
        <div>
                <Form className='sendMessage' onSubmit={onSubmit}>

                    <Form.Group className='grMess1'>

                        <Form.Control type='text' placeholder='Text'
                            value = {poruka} onChange= { (e) => 
                            setPoruku(e.target.value) }/>
                    </Form.Group>

                        <Form.Group className='grMess2'>
                        <Button variant="dark" type='submit' className='messButton'>
                        <img className="PanelIcon" src={SendIcon}/>
                        </Button>
                            </Form.Group>
                </Form>
                <div className='notifBox' id='notifBox'>
                { (objave.length == 0) && <div className='prazno'><h2>... No posts in this team just yet! :)</h2></div> }

            {objave.map(objava => {
                return (        
                <div  key={objava.id} style={{margin:"20px"}}>
                    <img className="PanelIcon" src={UserIcon}/> {objava.korisnik.username}
                    <div className='myMessage'>{objava.poruka}</div>
                    {new Date(objava.vreme).toUTCString()}
                </div>
                )
            })}
            </div>
        </div>
    )
}

export default Notifications