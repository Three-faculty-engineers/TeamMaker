import React from 'react'
import { useEffect, useState } from 'react'
import { Form, Button } from 'react-bootstrap'
import UserIcon from '../img/icons8-person-30.png'
import SendIcon from '../img/icons8-email-send-30.png'


function Chat(props) {
    const [poruke, setPoruke] = useState([])
    const [username, setUsername] = useState("")
    const [tekst, setTekst] = useState("")

    useEffect(() => {
        setUsername(sessionStorage.getItem("username"))
        
        const request = {
            method: 'GET',
            headers: {'Authorization': `bearer ${sessionStorage.getItem("jwt")}`}   
        }
        
        fetch(`https://localhost:7013/Poruka/GetPorukeIzmedjuDvaKor/${props.kor2ID}`, request).then(response => {
            if(response.ok)
            response.json().then((porukelocal) => {
                setPoruke(porukelocal)      
                var div = document.getElementById("messageBox");
                div.scrollTop = div.scrollHeight;
            })
        })
    }, [props.kor2ID])

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

        fetch(`https://localhost:7013/Poruka/CreatePoruka/${props.username2}/${tekst}`, request).then(response => {
            if(response.ok)
            {
                alert("Msg sent")
            }
            return response.json();
        }).then(data => {
            setPoruke([...poruke, {korisnikSnd: {username: data.userSent}, id: "0", tekst: data.txt, vreme: new Date()}])
        })
      
      
    }

   

    return (
        <div>
            <div className='messageBox' id='messageBox'>
                {poruke.map((poruka) => (poruka.korisnikSnd.username == username) ? 
                    (<div className='myMessageWith' key={poruka.id}>
                        <img className="PanelIcon" src={UserIcon}/> YOU
                        <div className='myMessage'>{poruka.tekst}</div>
                        {new Date(poruka.vreme).toUTCString()}</div>) : 
                    (<div className='theirMessageWith' key={poruka.id}>
                        <img className="PanelIcon" src={UserIcon}/> {poruka.korisnikSnd.username}
                        <div className='theirMessage'>{poruka.tekst}</div>
                        {new Date(poruka.vreme).toUTCString()}</div>)) 
                }
            </div>
            <Form className='sendMessage' onSubmit={onSubmit}>
            <Form.Group className='grMess1'>
                    <Form.Control type='text' placeholder='Type here...' value = {tekst} onChange= { (e) => 
                                setTekst(e.target.value) } />
            </Form.Group>


            <Form.Group className='grMess2'>
                <Button variant="dark" className='messButton' type='submit'>
                  <img className="PanelIcon" src={SendIcon}/>
                </Button>
            </Form.Group>

            </Form>

        </div>
    )
}

export default Chat