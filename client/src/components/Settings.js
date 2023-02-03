import React from 'react'
import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import { useState, useEffect } from 'react'

function Settings(props) {

    const [password, setPassword] = useState("")
    const [password2, setPassword2] = useState("")
    const [image, setImage] = useState("")
    const [imageU, setImageU] = useState(null)

    useEffect(() => {
      
        const request = {
            method: "GET",
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`} 
        } 
        fetch('https://localhost:7013/Korisnik/getCurrentUserImage', request).then((response) => {
            if(response.ok)
                response.json().then((image) => {
                    setImageU(image.imageBase64)
                })
        })
    }, [])
    



    const onSubmit = (event) => {
        event.preventDefault()
    
        if(password != password2)
        {
            console.log("passwords do not match")
            return
        }
        const request = {
            method: "PUT",
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`, 'Content-Type': 'application/json'},
            body: JSON.stringify({ 'username': 'nista', 'sifra': password,
            'prosek': '1','fakultet': 'nista', 'indeks': '12312',
            'ime': 'nista', 'prezime': 'nista', 'email': 'nista'})
        } 
        fetch('https://localhost:7013/Korisnik/updateKorisnika', request).then(response => {
          if(response.ok)
          {
            alert("Password changed successfully")
            setPassword("")
            setPassword2("")
          }
        })  
    
    }
    
    const sendImage = (event) => {
        event.preventDefault()
        const fd = new FormData();
        fd.append('name', 'slika')
        fd.append('file', image)
        const requestImage = {
            method: "PUT",
            headers: { 'Authorization': `bearer ${sessionStorage.getItem("jwt")} `},
            body: fd
        } 
         fetch('https://localhost:7013/Korisnik/uploadImage', requestImage).then(response => {
            if(response.ok)
                response.json().then((image) => {
                    console.log(image.imageBase64)
                    setImageU(image.imageBase64)
                
                })
        })   
    }



    return (      
    <div>

            <div>
                <img
                    src={imageU}
                    alt={""}
                    width="100"
                    height="100"
                    text-align="left"
                    style={{ display: "block" }}
                    className="img-thumbnail"
                    />
            </div>              
            <h3>{props.username}</h3>
   
                <Form className='settingsForm' onSubmit={sendImage}> 

                <Form.Group style={{padding:"10px"}}>

                    <Form.Label>Upload image </Form.Label>

                    <Form.Control type='file'  onChange= { (e) => setImage(e.target.files[0]) }/>

                </Form.Group>  

                <Button variant="dark" className='button' type='submit'>Change</Button>

                </Form>
                
                <Form className='settingsForm' onSubmit={onSubmit}> 

                    <Form.Group style={{padding:"10px"}}>

                        <Form.Label>Enter new password </Form.Label>

                        <Form.Control type='password' placeholder='●●●●●●●●●' value = {password} onChange= { (e) => 
                            setPassword(e.target.value) } />

                    </Form.Group> 

                    <Form.Group style={{padding:"10px"}}>

                        <Form.Label>Confirm new password </Form.Label>

                        <Form.Control type='password' placeholder='●●●●●●●●●' value = {password2} onChange= { (e) => 
                            setPassword2(e.target.value) } />

                    </Form.Group>  

                    <Button variant="dark" className='button' type='submit'>Change</Button>

                </Form>

            </div>
    )
}

export default Settings