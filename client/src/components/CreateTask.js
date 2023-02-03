import React, {useState, useEffect} from 'react'
import { Form, Button } from 'react-bootstrap'
import LeftArrow from '../img/icons8-prev-48.png'

function CreateTask(props) {
  
  const [ime, setIme] = useState("")
  const [opis, setOpis] = useState("")
  const [status, setStatus] = useState(0)

  // const onClick = () => {
  //   alert("TEST");
  // }
  const onSubmit = (event) => {
    event.preventDefault()

    if(ime == "" || opis == "")
      return

    const request = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' , 'Authorization': `bearer ${sessionStorage.getItem("jwt")}`},
      body: JSON.stringify({'ime' : ime, 'opis' : opis, 'status' : status})
    } 

    fetch('https://localhost:7013/Task/CreateTask/' + props.teamID, request).then(response => {
      if(response.ok)
          response.json().then((task) => {
            props.addTask(task);
            
          })
          
    })


  }
  


  return (
      <Form className='taskForm form2' onSubmit={onSubmit}>

        <Form.Group className='backlog'>

          <h3>Free task list:</h3>
          <div> {props.array1.map( (task) => ( <div className = "TaskOnList" key={task.id} onClick={() => {props.onKlik(task)}}> <img className ="arrowLeft" src={LeftArrow} alt="<" /> <p>{task.ime} - {task.opis}</p></div> ) )} </div> 

        </Form.Group>

        <Form.Group className='form-cont'>
            <Form.Label>Name</Form.Label>

            <Form.Control type='text' placeholder='ime'
                value = {ime} onChange= { (e) => 
                setIme(e.target.value) }/>
        </Form.Group>

        <Form.Group className='form-cont'>
            <Form.Label>Description</Form.Label>

            <Form.Control type='text' placeholder='opis'
                value = {opis} onChange= { (e) => 
                setOpis(e.target.value) }/>
        </Form.Group>

        <Button variant='dark' className='button' type='submit'>Create</Button>

      </Form>

  )
}

export default CreateTask