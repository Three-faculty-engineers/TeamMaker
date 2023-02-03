import React from 'react'
import { useState, useEffect } from 'react'
import { Form, Button } from 'react-bootstrap'
import SendIcon from '../img/icons8-email-send-30.png'

function CreateMsg(props) {
    const [username, setUsername] = useState("")
    const [tekst, setTekst] = useState("")

    const onSubmit = (event) => {
        event.preventDefault()

        if(!username)
        {
            alert('Izostavili ste username')
            return
        }

        const request = {
            method: 'POST',
            headers: {'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}
          
        } 
        
       

        fetch(`https://localhost:7013/Poruka/CreatePoruka/${username}/${tekst}`, request).then(response => {
            if(response.ok)
                response.json().then((kor) => {
                    props.setShowState(2)
                    props.setKor2ID(kor.id)
                    props.setUsername2(username)
                })
        })
      
      
    }

    return (
        <Form className='newChatForm' onSubmit={onSubmit}>

            <Form.Group style={{padding:"10px"}}>
                <Form.Label>To: </Form.Label>

                <Form.Control type='text' placeholder='User...' value = {username} onChange= { (e) => 
                        setUsername(e.target.value) } />
            </Form.Group>

            <Form.Group style={{padding:"10px"}}>    
            <Form.Control type='text' placeholder='Type message here...' value = {tekst} onChange= { (e) => 
                            setTekst(e.target.value) } />
            </Form.Group>
            <Button variant="dark" style={{width:"50%"}} type='submit' className='button'>
            <img className="PanelIcon" src={SendIcon}/> Send
            </Button>

        </Form>

    )
}

export default CreateMsg