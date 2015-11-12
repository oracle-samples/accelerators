<rn:block id='TextInput-postLabel'>
    <div id="rn_Line">
        <div id="rn_<?= $this->instanceID ?>_ValidationResultDisplay" class="rn_Hidden"></div>
        <div id="rn_<?= $this->instanceID ?>_ValidationQuestion" class="rn_Hidden">
        <input type="checkbox" id="rn_<?= $this->instanceID ?>_ValidateCheckbox"/>
        Do you want to use it?
        </div>
    </div>
</rn:block>

<!-- moar blocks used to be here -->

<rn:block id='TextInput-bottom'>
    <button class="rn_VerifyButton" id="rn_<?= $this->instanceID; ?>_VerifySubmit" type="button"><?= $this->data['attrs']['label_button'] ?></button>
    <span id="rn_<?= $this->instanceID; ?>_Loading">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>
</rn:block>



